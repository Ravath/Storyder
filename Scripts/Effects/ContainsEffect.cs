using System;
using System.Collections.Generic;
using Weaver.Heroes.Body;
using Weaver.Heroes.Body.Value;
using Weaver.Heroes.Destiny;

namespace Storyder;


public class ContainsEffect : VariableEffect, ICondition
{
    private bool _lastResult;
    public StoryderEffect ifTrue;
    public StoryderEffect ifFalse;

    public override string EffectName => " C ";

    public ContainsEffect(string modulePath, string strVal) : base(modulePath, strVal)
    {
    }

    bool ICondition.IsTrue => IsTrue();

    public static ConditionEffect Create(string[] args)
    {
        CheckNumberArguments(args,2,2);
        
        string strPath = args[0].Trim();
        string strVal = args[1].Trim();
        ContainsEffect varAccess = new(strPath, strVal) { };
        ConditionEffect ret = new()
        {
            condition = varAccess
        };

        return ret;
    }

    public bool IsTrue() {
        Module m = Game.Static.BaseModule.GetRegisteredByPath<Module>(ModulePath);
        if (m is ValueModule<List<int>> vli) {
            _lastResult = vli.Value.Contains(GetIntVal);
            return _lastResult;
        }
        if (m is ValueModule<List<string>> vls) {
            _lastResult = vls.Value.Contains(StrVal);
            return _lastResult;
        }
        Log.LogErr("CONTAINS test : '{0}' not found.", ModulePath);
        return false;
    }

    public string ToMacro()
    {
        return string.Format("{0} CONTAINS {1}", ModulePath, StrVal);
    }

    public override void ActuateInt(StoryReader storyReader, int val)
    {
        throw new NotImplementedException();
    }

    public override void ActuateStr(StoryReader storyReader, string val)
    {
        throw new NotImplementedException();
    }
}