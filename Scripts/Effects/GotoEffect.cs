using System;
using Weaver.Tales;

namespace Storyder;


public class GotoEffect : StoryderEffect, IParagraphLink
{
    public string ParagraphLabel { get; set; }
    public StoryParagraph Next { get; set; }

    public static GotoEffect Create(string[] args)
    {
        CheckNumberArguments(args,1,1);
        
        string strarg = args[0].Trim();
        GotoEffect ret = new()
        {
            ParagraphLabel = strarg
        };

        return ret;
    }

    public override void Actuate(StoryReader storyReader)
    {
        storyReader.SetStoryChunk(Next);
    }
}