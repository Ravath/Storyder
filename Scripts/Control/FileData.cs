using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Godot;

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
                foreach(string effectString in effects.Split(';'))
                {
                    if(string.IsNullOrWhiteSpace(effectString))
                        continue;
                    
                    string[] args = effectString.Split(':');
                    string effectName = args[0].Trim().ToUpper();

                    StoryEffect row_effect = GetEffectByCommand(effectName, args[1..]);

                    if(row_effect != null)
                        row_paragraph.Effects.Add(row_effect);
                    else
                        Log.LogErr("Row {0}. Could not parse effect name '{1}'.",
                            row.RowNumber(),
                            effectName);
                }

                // Post-Effects
                foreach(string effectString in posteffects.Split(';'))
                {
                    if(string.IsNullOrWhiteSpace(effectString))
                        continue;
                    
                    string[] args = effectString.Split(':');
                    string effectName = args[0].Trim().ToUpper();

                    StoryEffect row_effect = GetEffectByCommand(effectName, args[1..]);

                    if(row_effect != null)
                        row_paragraph.PostEffects.Add(row_effect);
                    else
                        Log.LogErr("Row {0}. Could not parse post-effect name '{1}'.",
                            row.RowNumber(),
                            effectName);
                }

                // Add to Story
                ret_story.AddChunk(row_paragraph);
            }

            // Properly link choices
            foreach(var paragraph in ret_story.Paragraphs)
            {
                foreach(var choice in paragraph.Choices)
                {
                    if(ret_story.HasChunk(choice.Label))
                        choice.Next = ret_story.GetChunk(choice.Label);
                    else
                        Log.LogErr("Paragraph {0}. Can't find paragraph Label : '{1}'.",
                            paragraph.Label,
                            choice.Label);
                }
            }

            return ret_story;
        }

        public static StoryEffect GetEffectByCommand(string commandName, string[] arguments)
        {
            StoryEffect ret_effect = null;

            // Instanciate the effect actuator using the command name.
            switch(commandName)
            {
                case "PICT" :
                    ret_effect = PictureEffect.Create(arguments);
                    break;
                case "MUSIC" :
                    ret_effect = MusicEffect.Create(arguments);
                    break;
                case "TEXTONLY" :
                    ret_effect = TextOnlyEffect.Create(arguments);
                    break;
            }

            return ret_effect;
        }
    }
}