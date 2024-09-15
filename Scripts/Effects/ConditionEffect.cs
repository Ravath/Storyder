using System;
using System.Collections.Generic;
using System.Diagnostics;
using Weaver.Heroes.Destiny;
using Weaver.Heroes.Luck;

namespace Storyder;

public interface ICommandArborescence
{
    public int SubNumber { get; }
    public void AddSubEffects(StoryderEffect[] subs);
    public void GetSubEffects<T>(IList<T> subeffects);
}

public class ConditionEffect : StoryderEffect, ICommandArborescence
{
    public StoryderEffect ifTrue;
    public StoryderEffect ifFalse;
    public ICondition condition;

    public static ConditionEffect Create(string[] args)
    {
        CheckNumberArguments(args,1,1);
        
        string strarg = args[0].Trim().Replace(" ", "");
        // Test the condition
        ICondition cd = Condition.Parse(strarg, Game.Static.BaseModule);
        if(cd == null)
        {
            Log.LogErr("ConditionEffect Creation : Can't parse Condition argument '{0}'.", strarg);
        }

        ConditionEffect ret = new()
        {
            condition = cd
        };

        return ret;
    }

    public override void Actuate(StoryReader storyReader)
    {
        if(condition.IsTrue) {
            storyReader.AppendText(" [ {0} : SUCCESS ]", condition.ToMacro());
            ifTrue?.Actuate(storyReader);
        } else {
            storyReader.AppendText(" [ {0} : FAILURE ]", condition.ToMacro());
            ifFalse?.Actuate(storyReader);
        }
    }

    public int SubNumber { get => 2; }

    public void AddSubEffects(StoryderEffect[] subs)
    {
        Debug.Assert(subs.Length == SubNumber);
        ifTrue = subs[0];
        ifFalse = subs[1];
    }

    public void GetSubEffects<T>(IList<T> subeffects)
    {
        if(ifFalse is T g1)
            subeffects.Add(g1);
        if(ifFalse is ICommandArborescence c1)
            c1.GetSubEffects<T>(subeffects);
        if(ifTrue is T g2)
            subeffects.Add(g2);
        if(ifTrue is ICommandArborescence c2)
            c2.GetSubEffects<T>(subeffects);
    }
}