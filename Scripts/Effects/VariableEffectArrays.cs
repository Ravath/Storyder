using System;
using System.Collections.Generic;
using System.Linq;
using Weaver.Heroes.Body;
using Weaver.Heroes.Body.Value;

namespace Storyder;


public class ArrayAddEffect : VariableEffect
{
    public ArrayAddEffect(string modulePath, string strVal) : base(modulePath, strVal)
    {
    }

    public override string EffectName => "ADD[]";

    public static ArrayAddEffect Create(string[] args)
    {
        CheckNumberArguments(args,2,2);
        
        string strPath = args[0].Trim();
        string strVal = args[1].Trim();
        ArrayAddEffect ret = new(strPath, strVal);

        return ret;
    }

    public override void ActuateInt(StoryReader storyReader, int val)
    {
        ValueModule<List<int>> vi = GetModule<ValueModule<List<int>>>(ModulePath);
        vi.BaseValue.Add(val);
    }

    public override void ActuateStr(StoryReader storyReader, string val)
    {
        ValueModule<List<string>> vi = GetModule<ValueModule<List<string>>>(ModulePath);
        vi.BaseValue.Add(val);
    }
}

public class ArraySetEffect : VariableEffect
{
    public ArraySetEffect(string modulePath, string strVal) : base(modulePath, strVal)
    {
    }

    public override string EffectName => "SET[]";

    public static ArraySetEffect Create(string[] args)
    {
        CheckNumberArguments(args,2,2);
        
        string strPath = args[0].Trim();
        string strVal = args[1].Trim();
        ArraySetEffect ret = new(strPath, strVal);

        return ret;
    }

    public override void ActuateInt(StoryReader storyReader, int val)
    {
        var m = CreateIfNotExist<List<int>>(new());
        m.BaseValue.Clear();
        m.BaseValue.Add(val);
    }

    public override void ActuateStr(StoryReader storyReader, string val)
    {
        var m = CreateIfNotExist<List<string>>(new());
        m.BaseValue.Clear();
        m.BaseValue.Add(val);
    }
}

public class ArrayRemoveEffect : VariableEffect
{
    public ArrayRemoveEffect(string modulePath, string strVal) : base(modulePath, strVal)
    {
    }

    public override string EffectName => "REM[]";

    public static ArrayRemoveEffect Create(string[] args)
    {
        CheckNumberArguments(args,2,2);
        
        string strPath = args[0].Trim();
        string strVal = args[1].Trim();
        ArrayRemoveEffect ret = new(strPath, strVal);

        return ret;
    }

    public override void Actuate(StoryReader storyReader)
    {
        Module m = Game.Static.BaseModule.GetRegisteredByPath<Module>(ModulePath);
        m.Unregister(m.Parent);
    }

    public override void ActuateInt(StoryReader storyReader, int val)
    {
        var m = GetModule<ValueModule<List<int>>>(ModulePath);
        m.BaseValue.Remove(val);
    }

    public override void ActuateStr(StoryReader storyReader, string val)
    {
        var m = GetModule<ValueModule<List<string>>>(ModulePath);
        m.BaseValue.Remove(val);
    }
}