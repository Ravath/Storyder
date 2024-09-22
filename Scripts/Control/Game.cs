using System;
using System.Collections.Generic;
using Weaver.Heroes.Body;
using Weaver.Heroes.Body.Value;

namespace Storyder;

public class Game
{
    public static Game _instance;
    public static Game Static {
        get {
            _instance ??= new Game();
            return _instance;
        }
    }
    public Module BaseModule { get; private set; } = new("Base");
    public Module WorkModule { get; private set; } = new("W");
    public ValueModule<List<string>> Flags { get; private set; } = new("Flags", new List<string>());
    public const string HeroModuleName = "Hero";
    public Module HeroModule { get => BaseModule.GetRegistered(HeroModuleName); }

    public ISystemImplementation System { get; private set; }

    private Game() {
        System = new FightingFantasySystem.System();
    }

    public void Init()
    {
        BaseModule.UnregisterAll();
        BaseModule.Register(WorkModule);
		WorkModule.UnregisterAll();
        WorkModule.Register(Flags);
        Flags.UnregisterAll();
        Flags.BaseValue.Clear();
        System.Init();
    }
}
