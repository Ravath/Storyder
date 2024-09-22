using System;
using System.Collections.Generic;
using System.Linq;
using ClosedXML.Excel;
using Godot;
using Weaver.Tales;

namespace Storyder
{
    public static class FileData
    {
        public static void CreateDefaultExcel(string name)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Story");
            worksheet.Cell("A1").Value = "ID";
            worksheet.Cell("B1").Value = "Text";
            worksheet.Cell("C1").Value = "DevNotes";
            worksheet.Cell("D1").Value = "Choices";
            worksheet.Cell("E1").Value = "Effects";
            worksheet.Cell("F1").Value = "PostEffects";
            worksheet.FirstRow().Style
                .Font.SetBold()
                .Fill.SetBackgroundColor(XLColor.CornflowerBlue)
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            workbook.SaveAs(name);
        }

        /// <summary>
        /// Get the value of the cell as a string.
        /// Can't return null. Empty string at least.
        /// Already trimmed.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="columnNumber"></param>
        /// <returns>A trimmed string.</returns>
        public static String GetCellString(this IXLRow row, int columnNumber)
        {
            return row.Cell(columnNumber).Value.ToString()?.Trim() ?? "";
        }

        public static Story ReadFromExcel(string fileName)
        {
            using var workbook = new XLWorkbook(fileName);
            Story ret_story = new();

            foreach(var row in workbook.Worksheet(1).Rows().Skip(1))
            {
                // Get row values
                string label = row.GetCellString(1);
                string text = row.GetCellString(2);
                string devNotes = row.GetCellString(3);
                string choices = row.GetCellString(4);
                string effects = row.GetCellString(5);
                string posteffects = row.GetCellString(6);

                // Check and clean values
                // -- label
                if(string.IsNullOrEmpty(label)) {
                    Log.LogInfo("Skipped row '{0}'.", row.RowNumber());
                    break;
                }
                // -- text
                // Nothing
                // -- devNotes
                // Nothing
                // -- choices
                choices = choices.Replace("\n","");
                choices = choices.Replace("\r","");
                // choices = choices.Replace("\"","");
                // choices = choices.Replace("”","");
                // -- effects
                effects = effects.Replace("\n","");
                effects = effects.Replace("\r","");
                // effects = effects.Replace(" ","");
                // -- posteffects
                posteffects = posteffects.Replace("\n","");
                posteffects = posteffects.Replace("\r","");

                // Init the paragraph
                StoryParagraph row_paragraph = new()
                {
                    Label = label,
                    Text = text,
                    DevNotes = devNotes
                };

                // Choices
                var paragraphChoices = choices.Split(';');
                foreach(string choiceString in paragraphChoices)
                {
                    if(string.IsNullOrWhiteSpace(choiceString))
                        continue;
                    
                    string[] choiceArgs = choiceString.Split(':');

                    if(choiceArgs.Length < 1 || choiceArgs.Length > 2)
                    {
                        Log.LogErr("Row {0}. Link choice has wrong number of arguments : '{1}'.",
                            row.RowNumber(),
                            choiceArgs.Join(":"));
                        break; // Dont continue to parse this choice.
                    }
                    else if(choiceArgs.Length != 2)
                    {
                        // If no text and only one Label, than use "Continue" as text.
                        if(paragraphChoices.Length == 1)
                        {
                            choiceArgs = new string[] { choiceArgs[0], "Continue",};
                        } else {
                            Log.LogErr("Row {0}. Incompatible Choice implementation :  '{1}'.",
                                row.RowNumber(),
                                choiceArgs.Join(":"));
                            break; // Dont continue to parse this choice.
                        }
                    }

                    // Add the new choice to the paragraph
                    string choiceId = choiceArgs[0].Trim();
                    string choiceText = choiceArgs[1].Trim();
                    row_paragraph.Choices.Add(new StoryChoice()
                    {
                        Label = choiceId,
                        Text = choiceText
                    });
                }

                // Effects
                var convertedEffects = GetEffects(effects.Split(';'), row);
                row_paragraph.Effects.AddRange(convertedEffects);

                // Post-Effects
                convertedEffects.Clear();
                convertedEffects = GetEffects(posteffects.Split(';'), row);
                row_paragraph.PostEffects.AddRange(convertedEffects);

                // Add to Story
                ret_story.AddChunk(row_paragraph);
            }

            // Post process the paragraphs
            foreach(var paragraph in ret_story.Paragraphs)
            {
                PostProcessParagraph(paragraph, ret_story);
            }

            return ret_story;
        }

        /// <summary>
        /// Link the labels to paragraphs and if conditions to commnds.
        /// </summary>
        /// <param name="paragraph"></param>
        /// <param name="ret_story"></param>
        public static void PostProcessParagraph(StoryParagraph paragraph, Story ret_story)
        {
            // Link choices label to paragraphs
            foreach(var choice in paragraph.Choices.OfType<StoryChoice>())
            {
                if(ret_story.HasChunk(choice.Label))
                    choice.Next = ret_story.GetChunk(choice.Label);
                else
                    Log.LogErr("Paragraph {0}. Can't find paragraph Label : '{1}' in choices.",
                        paragraph.Label,
                        choice.Label);
            }

            // Get the effects that can be linked
            var links = paragraph.PostEffects
                .Concat(paragraph.Effects)
                .OfType<IParagraphLink>();
            List<IParagraphLink> cdtLinks = new();
            
            // Get the effects that can be linked, but are hidden in the condition structure
            foreach (var cdt in paragraph.PostEffects
                .Concat(paragraph.Effects)
                .OfType<ICommandArborescence>())
            {
                cdt.GetSubEffects(cdtLinks);
            }
            
            // Link Labels to paragraphs
            foreach (var effect in links.Concat(cdtLinks))
            {
                if(ret_story.HasChunk(effect.ParagraphLabel))
                    effect.Next = ret_story.GetChunk(effect.ParagraphLabel);
                else
                    Log.LogErr("Paragraph {0}. Can't find paragraph Label : '{1}' in GOTO effect.",
                        paragraph.Label,
                        effect.ParagraphLabel);
            }
        }

        public static List<StoryderEffect> GetEffects(string[] effects, IXLRow row)
        {
            List<StoryderEffect> ret = new();

            // Convert strings to implemented effects
            foreach(string effectString in effects)
            {                
                string[] args = effectString.Split(':');
                string effectName = args[0].Trim().ToUpper();
                try {
                    StoryderEffect row_effect = GetEffectByCommand(effectName, args[1..]);
                    ret.Add(row_effect);

                    // Error only if some text.
                    // Empty string must be dealt with, for the ICommandArborescence to work properly.
                    if(row_effect == null && !string.IsNullOrWhiteSpace(effectName))
                        Log.LogErr("Row {0}. Could not parse effect '{1}'.",
                            row.RowNumber(),
                            effectName);
                } catch (Exception e) {
                    Log.LogErr("Row {0}. Exception when parsing effect '{1}' : '{2}'",
                            row.RowNumber(),
                            effectName,
                            e.Message);
                }
            }

            // Link the arborescence effects to following commands
            for(int i = ret.Count-1; i >= 0; i--)
            {
                if(ret[i] is ICommandArborescence ce)
                {
                    // Check if the last command to link exists.
                    if(i+ce.SubNumber >= ret.Count)
                        Log.LogErr("Row {0}. Could not link Arborecence Command {1} to following Effect. No command at position {2}.",
                            row.RowNumber(),
                            ce.GetType().Name,
                            i+ce.SubNumber);
                    // Assume if we can link to the last command, we can link the other ones to.
                    else {
                        // Give the commands to the arborescence
                        ce.AddSubEffects(ret.GetRange(i+1, ce.SubNumber).ToArray());
                        // Remove the given commands
                        for(int j = 0; j < ce.SubNumber; j++)
                            ret.RemoveAt(i+1);
                    }
                }
            }

            // THEN, remove the null effects due to empty strings
            ret.RemoveAll(x => x == null);

            return ret;
        }

        public static StoryderEffect GetEffectByCommand(string commandName, string[] arguments)
        {
            StoryderEffect ret_effect = null;

            // Instanciate the effect actuator using the command name.
            switch(commandName)
            {
                case "GOTO" :
                    ret_effect = GotoEffect.Create(arguments);
                    break;
                case "APPLY" :
                    ret_effect = ApplyEffect.Create(arguments);
                    break;
                case "CHOICE" :
                    ret_effect = AddChoiceEffect.Create(arguments);
                    break;
                case "APPEND" :
                    ret_effect = AppendTextEffect.Create(arguments);
                    break;
                case "TEXTONLY" :
                    ret_effect = TextOnlyEffect.Create(arguments);
                    break;
                case "PICT" :
                    ret_effect = PictureEffect.Create(arguments);
                    break;
                case "MUSIC" :
                    ret_effect = MusicEffect.Create(arguments);
                    break;
                case "ADD" :
                    goto case "SUM";
                case "SUM" :
                    ret_effect = SumVarEffect.Create(arguments);
                    ret_effect.PublicTrace = true;
                    break;
                case "SET" :
                    ret_effect = SetVarEffect.Create(arguments);
                    ret_effect.PublicTrace = true;
                    break;
                case "REM" :
                    ret_effect = RemoveVarEffect.Create(arguments);
                    ret_effect.PublicTrace = true;
                    break;
                case "ADD[]" :
                    ret_effect = ArrayAddEffect.Create(arguments);
                    ret_effect.PublicTrace = true;
                    break;
                case "SET[]" :
                    ret_effect = ArraySetEffect.Create(arguments);
                    ret_effect.PublicTrace = true;
                    break;
                case "REM[]" :
                    ret_effect = ArrayRemoveEffect.Create(arguments);
                    ret_effect.PublicTrace = true;
                    break;
                case "IF" :
                    ret_effect = ConditionEffect.Create(arguments);
                    ret_effect.PublicTrace = true;
                    break;
                case "CONTAINS" :
                    ret_effect = ContainsEffect.Create(arguments);
                    ret_effect.PublicTrace = true;
                    break;
            }

            // If not general command, should be a system specific command. 
            ret_effect ??= Game.Static.System.ParseEffectCommand(commandName, arguments);

            return ret_effect;
        }
    }
}