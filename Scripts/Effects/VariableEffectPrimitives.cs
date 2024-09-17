using System;
using Weaver.Heroes.Body;
using Weaver.Heroes.Body.Value;

namespace Storyder;


public class SumVarEffect : VariableEffect
{
    public SumVarEffect(string modulePath, string strVal) : base(modulePath, strVal)
    {
    }

    public static SumVarEffect Create(string[] args)
    {
        CheckNumberArguments(args,2,2);
        
        string strPath = args[0].Trim();
        string strVal = args[1].Trim();
        SumVarEffect ret = new(strPath, strVal);

        return ret;
    }

    public override void ActuateInt(StoryReader storyReader, int val)
    {
        ValueModule<int> vi = GetModule<ValueModule<int>>();
        vi.BaseValue += val;
    }

    public override void ActuateStr(StoryReader storyReader, string val)
    {
        ValueModule<string> vi = GetModule<ValueModule<string>>();
        vi.BaseValue += val;
    }
}

public class SetVarEffect : VariableEffect
{
    public SetVarEffect(string modulePath, string strVal) : base(modulePath, strVal)
    {
    }

    public static SetVarEffect Create(string[] args)
    {
        CheckNumberArguments(args,2,2);
        
        string strPath = args[0].Trim();
        string strVal = args[1].Trim();
        SetVarEffect ret = new(strPath, strVal);

        return ret;
    }

    public override void ActuateInt(StoryReader storyReader, int val)
    {
        var m = CreateIfNotExist<int>(val);
        m.BaseValue = val;
    }

    public override void ActuateStr(StoryReader storyReader, string val)
    {
        var m = CreateIfNotExist<string>(val);
        m.BaseValue = val;
    }
}

public class RemoveVarEffect : StoryderEffect
{
    public string ModulePath { get; set; }

    public static RemoveVarEffect Create(string[] args)
    {
        CheckNumberArguments(args,1,1);
        
        string strPath = args[0].Trim();

        RemoveVarEffect ret = new()
        {
            ModulePath = strPath
        };

        return ret;
    }

    public override void Actuate(StoryReader storyReader)
    {
        Module m = Game.Static.BaseModule.GetRegisteredByPath<Module>(ModulePath);
        m.Unregister(m.Parent);
    }
}