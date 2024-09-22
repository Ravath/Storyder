using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using DocumentFormat.OpenXml.Office2016.Drawing.Command;
using Godot;
using Weaver.Heroes.Destiny;
using Weaver.Tales;

namespace Storyder;

/// <summary>
/// The base class for Storyd effects implemented for Storyder.
/// </summary>
public abstract class StoryderEffect : IStoryEffect
{
    public bool PublicTrace {get; set;} = false;

    public static void CheckNumberArguments(string[] args, int min, int max) {
        if(args.Length < min) throw new ArgumentException("Expected at least "+min+" arguments.");
        if(args.Length > max) throw new ArgumentException("Expected at most "+max+" arguments.");
    }

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
    public abstract string GetTrace();

    public void Actuate(object target)
    {
        StoryReader sr = (StoryReader) target;
        Actuate(sr);
        if(PublicTrace) {
            sr.AppendText(string.Format("  [ {0} ]\n", GetTrace()));
        }
    }
}