using System;
using Weaver.Tales;

namespace Storyder;


public class AddChoiceEffect : StoryderEffect, IParagraphLink
{
    public string ChoiceText { get; set; }
    public string ParagraphLabel { get; set; }
    public StoryParagraph Next { get; set; }

    public static AddChoiceEffect Create(string[] args)
    {
        CheckNumberArguments(args,1,2);
        
        string strLabel = args[0].Trim();
        string strChoice = "Continue";
        if(args.Length == 2)
            strChoice = args[1].Trim();
        AddChoiceEffect ret = new()
        {
            ParagraphLabel = strLabel,
            ChoiceText = strChoice
        };

        return ret;
    }

    public override void Actuate(StoryReader storyReader)
    {
        storyReader.AppendChoice(new StoryChoice() {
            Next = Next,
            Label = ParagraphLabel,
            Text = ChoiceText
        });
    }

    public override string GetTrace()
    {
        return string.Format("{0} => {1}", ParagraphLabel, ChoiceText);
    }
}