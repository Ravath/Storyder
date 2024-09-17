using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Weaver.Heroes.Body;
using Weaver.Heroes.Body.Value;
using Weaver.Heroes.Luck;

namespace Storyder.FightingFantasySystem;


public class System : ISystemImplementation
{
    private Agent _hero = new(Game.HeroModuleName);
    public Agent Hero { get => _hero; }

    public bool IsGameFinished => Hero.IsDead;

    public void DoEffectChecks(StoryReader storyReader)
    {
        if(IsGameFinished)
        {
            storyReader.SetEndGame(false);
        }
    }

    public void Init()
    {
        Module baseModule = Game.Static.BaseModule;
        if(baseModule.HasRegistered(_hero))
            baseModule.Unregister(_hero);

        // Init Main Character
        _hero = new("Hero");
        baseModule.Register(_hero);
    }

    public StoryderEffect ParseEffectCommand(string commandName, string[] arguments)
    {
        StoryderEffect ret_effect = null;

        // Instanciate the effect actuator using the command name.
        switch(commandName)
        {
            case "LUCK" :
                ret_effect = LuckEffect.Create(arguments, (System)Game.Static.System);
                break;
            case "COMBAT" :
                ret_effect = CombatEffect.Create(arguments, (System)Game.Static.System);
                break;
        }

        return ret_effect;
    }
}
