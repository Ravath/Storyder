using System;
using System.Diagnostics;
using System.IO;

namespace Storyder;


public class PictureEffect : StoryderEffect
{
    public FileInfo filepath;

    public static PictureEffect Create(string[] args)
    {
        Debug.Assert(args.Length > 0);
        
        string filepath = args[0];
        PictureEffect ret = new()
        {
            filepath = GetPictureFile(filepath)
        };

        return ret;
    }

    public override void Actuate(StoryReader storyReader)
    {
        storyReader.HidePicturePanel(false);
        storyReader.SetPicture(filepath);
    }

    public override string GetTrace()
    {
        return "LOAD PICTURE : " + filepath.FullName;
    }
}