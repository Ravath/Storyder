using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ClosedXML.Excel;
using Godot;

namespace Storyder
{
    public class Story
    {
        private List<StoryParagraph> _paragraphs { get; } = new List<StoryParagraph>();
        private Dictionary<string, StoryParagraph> _paragraphsIndex = new Dictionary<string, StoryParagraph>();
        

        public string Name { get; set;}
        public string Description { get; set;}
        // TODO Creation Date, Author(s) and other credits, language, 
        // TODO Combat system, default character sheet and character creation
        public IEnumerable<StoryParagraph> Paragraphs { get { return _paragraphs; } }


        public void AddChunk(StoryParagraph chunk)
        {
            _paragraphsIndex.Add(chunk.Label, chunk);
            _paragraphs.Add(chunk);
        }

        public bool HasChunk(string id)
        {
            return _paragraphsIndex.ContainsKey(id);
        }

        public StoryParagraph GetChunk(string id)
        {
            return _paragraphsIndex[id];
        }
    }
}
    