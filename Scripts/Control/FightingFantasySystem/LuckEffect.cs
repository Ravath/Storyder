using System;
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
            return system.Hero.LuckTest();
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

    public string ToMacro()
    {
        return string.Format("Luck({0})", system.Hero.Luck);
    }
}
