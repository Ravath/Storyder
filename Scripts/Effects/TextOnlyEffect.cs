using System;

namespace Storyder;


public class TextOnlyEffect : StoryderEffect
{
    public bool setTextonly;
    public static TextOnlyEffect Create(string[] args)
    {
        CheckNumberArguments(args,0,1);

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