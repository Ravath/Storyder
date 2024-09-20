using System;
using System.Diagnostics;
using System.Linq;
using Godot;
using Weaver.Heroes.Body;
using Weaver.Heroes.Body.Value;
using Weaver.Heroes.Luck;

namespace Storyder;

public enum VariableType {
    String,
    Int,
    Roll, // Actually an int
}

public abstract class VariableEffect : StoryderEffect
{
    public string ModulePath { get; set; }
    public string StrVal { get; set; }
    public VariableType Type { get; set; } = VariableType.String;

    public int GetIntVal {
        get {
            if(Type == VariableType.Roll)
            {
                rVal.Roll();
                if(negativeRoll)
                    return - rVal.NetResult;
                return rVal.NetResult;
            }
            return iVal;
        }
    }
    private int iVal;
    private readonly IRoll rVal;
    private bool negativeRoll = false;

    public VariableEffect(string modulePath, string strVal)
    {
        ModulePath = modulePath;
        StrVal = strVal;

        if(int.TryParse(strVal, out int val)) {
            // Its an int
            iVal = val;
            Type = VariableType.Int;
        } else if(strVal.StartsWith("-[") || strVal.StartsWith("[") && strVal.EndsWith("]") ) {
            // Its a Roll
            if(strVal.StartsWith("-[")) {
                rVal = Roll.Parse(strVal[2..^1]);
                negativeRoll = true;
            }else{
                rVal = Roll.Parse(strVal[1..^1]);
            }
            Type = VariableType.Roll;
        } else {
            // Assume its a boring string, do nothing
        }
    }

    public T GetModule<T>()
    {
        T m = Game.Static.BaseModule.GetRegisteredByPath<T>(ModulePath);
        Debug.Assert(m != null);
        return m;
    }

    public ValueModule<T> CreateIfNotExist<T>(T value)
    {
        return Game.Static.BaseModule.CreateValueModuleIfNotExist<T>(ModulePath, value);
    }

    public override void Actuate(StoryReader storyReader)
    {
        switch(Type) {
            case VariableType.String:
                ActuateStr(storyReader, StrVal);
                break;
            case VariableType.Roll:
                rVal.Roll();
                iVal = rVal.NetResult;
                if(negativeRoll)
                    iVal *= -1;
                goto case VariableType.Int;
            case VariableType.Int:
                ActuateInt(storyReader, iVal);
                break;
        }
    }

    public abstract void ActuateInt(StoryReader storyReader, int val);
    public abstract void ActuateStr(StoryReader storyReader, string val);
}