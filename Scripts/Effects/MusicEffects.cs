using System;
using System.Diagnostics;
using System.IO;

namespace Storyder;


public class MusicEffect : StoryderEffect
{
    public FileInfo filepath;

    public static MusicEffect Create(string[] args)
    {
        Debug.Assert(args.Length > 0);
        
        string filepath = args[0];
        MusicEffect ret = new()
        {
            filepath = GetMusicFile(filepath)
        };

        return ret;
    }

    public override void Actuate(StoryReader storyReader)
    {
        storyReader.SetMusic(filepath);
    }

    public override string GetTrace()
    {
        return "LOAD MUSIC : " + filepath.FullName;
    }
}