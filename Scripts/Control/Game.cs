using System;
using Weaver.Heroes.Body;

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

    public ISystemImplementation System { get; private set; }

    private Game() {
        System = new FightingFantasySystem.System();
    }

}
