using System;
using System.Collections.Generic;
using Weaver.Heroes.Body;
using Weaver.Heroes.Body.Value;
using Weaver.Heroes.Destiny;

namespace Storyder.FightingFantasySystem;

public class LuckEffect : ConditionEffect, ICondition
{
    System system;

    /// <summary>
    /// Be sure to not call this function multiple times, as it will decrease luck each time.
    /// </summary>
    public bool IsTrue {
        get {
            _isTrue = system.Hero.LuckTest();
            return _isTrue;
        }
    }

    public static LuckEffect Create(string[] args, System system)
    {
        CheckNumberArguments(args,0,0);
        
        LuckEffect ret = new() {
            system = system
         };
        ret.condition = ret;

        return ret;
    }

    public override string GetTrace()
    {
        return string.Format("Luck({0}) : {1}", system.Hero.Luck, _isTrue?"SUCCESS":"FAILURE");
    }

    public string ToMacro()
    {
        return string.Format("Luck({0})", system.Hero.Luck);
    }
}