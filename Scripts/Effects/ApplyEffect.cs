using System;
using Weaver.Tales;

namespace Storyder;

public class ApplyEffect : StoryderEffect, IParagraphLink
{
    public string ParagraphLabel { get; set; }
    public StoryParagraph Next { get; set; }

    public static ApplyEffect Create(string[] args)
    {
        CheckNumberArguments(args,1,1);
        
        string strarg = args[0].Trim();
        ApplyEffect ret = new()
        {
            ParagraphLabel = strarg
        };

        return ret;
    }

    public override void Actuate(StoryReader storyReader)
    {
		// Actuate Pre-Effects
		foreach (var effect in Next.Effects)
		{
			effect.Actuate(storyReader);
		}
    }

    public override string GetTrace()
    {
        return string.Format("APPLY {0}", ParagraphLabel);
    }
}
