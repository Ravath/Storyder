using System;

namespace Storyder
{
    public class StoryChoice
    {
        /// <summary>
        /// If no text, just "Continue".
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// The identifier of a StoryParagraph.
        /// </summary>
        public string Label { get; set; }
        /// <summary>
        /// Not Null.
        /// </summary>
        public StoryParagraph Next { get; set; }
    }
}