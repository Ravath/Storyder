using System;
using System.Diagnostics;
using System.IO;
using Godot;
using Weaver.Tales;

namespace Storyder;

public abstract class StoryEffect : IStoryEffect
{
    public string ScriptCommand;

    public static FileInfo GetPictureFile(string filename)
    {
        FileInfo filepath = new FileInfo(StoryderProperties.RessourcePath + "/" + filename);
        if(!filepath.Exists)
            throw new ArgumentException(string.Format("There is no file '{0}'.", filepath));
        if(filepath.Extension != ".jpg"
        && filepath.Extension != ".jpeg"
        && filepath.Extension != ".png"
        && filepath.Extension != ".webp")
        {
            // TODO : check godot documentation for real capability
            throw new ArgumentException(string.Format("Can't read file extension '{0}' as a picture.", filepath.Extension));
        }
        return filepath;
    }

    public static FileInfo GetMusicFile(string filename)
    {
        FileInfo filepath = new FileInfo(StoryderProperties.RessourcePath + "/" + filename);
        if(!filepath.Exists)
            throw new ArgumentException(string.Format("There is no file '{0}'.", filepath));
        if(filepath.Extension != ".wav"
        && filepath.Extension != ".ogg")
        {
            // TODO : check godot documentation for real capability
            throw new ArgumentException(string.Format("Can't read file extension '{0}' as an audio.", filepath.Extension));
        }
        return filepath;
    }

    public abstract void Actuate(StoryReader storyReader);

    public void Actuate(object target)
    {
        Actuate((StoryReader) target);
    }
}

public class PictureEffect : StoryEffect
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
}

public class MusicEffect : StoryEffect
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
}

public class TextOnlyEffect : StoryEffect
{
    public bool setTextonly;
    public static TextOnlyEffect Create(string[] args)
    {
        Debug.Assert(args.Length <= 1);
        bool option = true;
        if(args.Length == 1)
        {
            string strarg = args[0].Trim().ToLower();
            switch(strarg)
            {
                case "false":
                case "off":
                    option = false;
                    break;
                case "true":
                case "on":
                    option = true;
                    break;
                default :
                    Log.LogErr("Can't parse TextOnly argument '{0}'.", strarg);
                    break;
            }
        }
        TextOnlyEffect ret = new()
        {
            setTextonly = option
        };

        return ret;
    }

    public override void Actuate(StoryReader storyReader)
    {
        storyReader.HidePicturePanel(setTextonly);
    }
}