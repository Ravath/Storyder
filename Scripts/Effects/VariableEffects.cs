using System;
using System.Diagnostics;
using System.Linq;
using Godot;
using Weaver.Heroes.Body;
using Weaver.Heroes.Body.Value;

namespace Storyder;


public class SumVarEffect : StoryderEffect
{
    public string ModulePath { get; set; }
    public string StrVal { get; set; }
    public int IntVal { get; set; }

    public static SumVarEffect Create(string[] args)
    {
        CheckNumberArguments(args,2,2);
        
        string strPath = args[0].Trim();
        string strVal = args[1].Trim();

        SumVarEffect ret = new()
        {
            ModulePath = strPath,
            StrVal = strVal
        };

        if(int.TryParse(strVal, out int val)) {
            ret.IntVal = val;
        }

        return ret;
    }

    public override void Actuate(StoryReader storyReader)
    {
        Module m = Game.Static.BaseModule.GetRegisteredByPath<Module>(ModulePath);
        Debug.Assert(m != null);
        if(m is ValueModule<int> vi)
        {
            vi.BaseValue += IntVal;
        }else if(m is ValueModule<string> vs)
        {
            vs.BaseValue += StrVal;
        }

        storyReader.AppendText(" [ {0} : {1}{2} ]", ModulePath, IntVal>0?"+":"", StrVal);
    }
}

public class SetVarEffect : StoryderEffect
{
    public string ModulePath { get; set; }
    public string StrVal { get; set; }
    public int IntVal { get; set; }
    public bool isInt = false;

    public static SetVarEffect Create(string[] args)
    {
        CheckNumberArguments(args,2,2);
        
        string strPath = args[0].Trim();
        string strVal = args[1].Trim();

        SetVarEffect ret = new()
        {
            ModulePath = strPath,
            StrVal = strVal
        };

        if(int.TryParse(strVal, out int val)) {
            ret.IntVal = val;
            ret.isInt = true;
        }

        return ret;
    }

    public override void Actuate(StoryReader storyReader)
    {
        string[] path = ModulePath.Split('.');
        string lastModuleName = path[^1];
        
        Module m = Game.Static.BaseModule.GetRegisteredByPath<Module>(path.Join("."));
        
        if(!m.HasRegistered(lastModuleName))
        {
            if(isInt) {
                ValueModule<int> vali = new ValueModule<int>(lastModuleName, IntVal);
                vali.OnValueChanged += storyReader.ActualiseIntValues;
                m.Register(vali);
            }
            else {
                ValueModule<string> vals = new ValueModule<string>(lastModuleName, StrVal);
                vals.OnValueChanged += storyReader.ActualiseStringValues;
                m.Register(vals);
            }
        }
        else
        {
            m = m.GetRegisteredByPath<Module>(lastModuleName);
            if(m is ValueModule<int> vi)
            {
                vi.BaseValue = IntVal;
            }
            else if(m is ValueModule<string> vs)
            {
                vs.BaseValue = StrVal;
            }
        }
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