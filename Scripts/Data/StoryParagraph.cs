using System;
using System.Collections.Generic;

namespace Storyder
{
    public class StoryParagraph
    {
        public string Label { get; set; }
        public string Text { get; set; }
        public string DevNotes { get; set; }
        public List<StoryChoice> Choices { get; } = new List<StoryChoice>();
        public List<StoryEffect> PostEffects { get; } = new List<StoryEffect>();
    }
}