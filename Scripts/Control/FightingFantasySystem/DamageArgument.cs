using System;
using Godot;

namespace Storyder.FightingFantasySystem;


public class DamageArgument
{
    public Agent Dealer { get; set; }
    public Agent Target { get; set; }
    public int Damage { get; set; } = 2;
    public bool UsedLuck = false;

    internal string ToLuckChoice()
    {
        if(Target.IsHero)
            return string.Format("CHANCE : Diminuer les dégâts reçus de {1}.", Target.Name, Dealer.Name);
        else
            return string.Format("CHANCE : Augmenter les dégâts infligés à {0}.", Target.Name);
    }

    internal string ToParagraph()
    {
        if(Damage == 0)
        {
            return string.Format("{0} ne parvient pas a toucher {1}.", Dealer.Name, Target.Name);
        }
        return string.Format("{0} inflige {1} blessures à {2}.", Dealer.Name, Damage, Target.Name);
    }

    internal void ApplyDamage()
    {
        Target.Stamina -= Damage;
    }
}