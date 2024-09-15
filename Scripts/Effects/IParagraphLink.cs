using System;
using Weaver.Tales;

namespace Storyder;


public interface IParagraphLink
{
    string ParagraphLabel { get; }
    StoryParagraph Next { get; set; }
}