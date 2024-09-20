using System;

namespace Storyder;


public class AppendTextEffect : StoryderEffect
{
    public string Text { get; set; }

    public static AppendTextEffect Create(string[] args)
    {
        CheckNumberArguments(args,1,1);
        
        string strLabel = args[0].Trim().Replace("\\n", "\n");
        AppendTextEffect ret = new()
        {
            Text = strLabel
        };

        return ret;
    }

    public override void Actuate(StoryReader storyReader)
    {
        storyReader.AppendText(Text);
    }
}