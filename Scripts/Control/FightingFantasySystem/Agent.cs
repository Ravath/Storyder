using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using Weaver.Heroes.Body;
using Weaver.Heroes.Body.Value;
using Weaver.Heroes.Luck;

namespace Storyder.FightingFantasySystem;

public class Agent : Module
{
    private ValueModule<int> luck, strenght, ability;
    private ValueModule<int> start_luck, start_strenght, start_ability;
    private ValueModule<int> potion_luck, potion_strenght, potion_ability;
    private ValueModule<int> food;

    public string Name { get; set; } = "Vous";
    public int Ability { 
        get => ability.BaseValue; 
        set => ability.BaseValue = value; 
    }
    public int Strenght { 
        get => strenght.BaseValue; 
        set => strenght.BaseValue = value; 
    }
    public int Luck {
        get => luck.BaseValue; 
        set => luck.BaseValue = value; 
    }

    public bool IsDead {
        get => Strenght <= 0;
    }
    public bool IsHero { get { return ModuleName == System.HeroModuleName; } }


    public Agent(string moduleName) : base(moduleName)
    {
        Module start = Register(new Module("Start"));
        Module current = Register(new Module("Current"));
        Module potions = Register(new Module("Potions"));
        // Attributes
        ability = new ValueModule<int>("Ability", Roll.RollMacro("1d6+6"));
        strenght = new ValueModule<int>("Strenght", Roll.RollMacro("2d6+12"));
        luck = new ValueModule<int>("Luck", Roll.RollMacro("1d6+6"));
        current.Register(ability);
        current.Register(strenght);
        current.Register(luck);
        // Start values
        start_ability = new ValueModule<int>("Ability", ability.Value);
        start_strenght = new ValueModule<int>("Strenght", strenght.Value);
        start_luck = new ValueModule<int>("Luck", luck.Value);
        start.Register(start_ability);
        start.Register(start_strenght);
        start.Register(start_luck);
        // Potion counters
        potion_ability = new ValueModule<int>("Ability", 0);
        potion_strenght = new ValueModule<int>("Strenght", 0);
        potion_luck = new ValueModule<int>("Luck", 0);
        potions.Register(potion_ability);
        potions.Register(potion_strenght);
        potions.Register(potion_luck);
        // Food
        food = new ValueModule<int>("Food", 10);
        Register(food);
    }

    public bool LuckTest()
    {
        bool result = luck.Value >= Roll.RollMacro("2d6");
        luck.BaseValue -= 1;
        return result;
    }

    public bool TestLuckOnCombat(DamageArgument damage)
    {
        // To work, should not be called by ennemies, as they cant use luck.
        Debug.Assert(IsHero);
        damage.UsedLuck = true;

        if(LuckTest()) {
            if(damage.Target == this)
                damage.Damage -= 1; // Hero receives less damage
            else
                damage.Damage += 2; // Hero deals more damage
            return true;
        } else {
            if(damage.Target == this)
                damage.Damage += 1; // Hero receives more damage
            else
                damage.Damage -= 1; // Hero deals less damage
            return false;
        }
    }

    public void TakeDamage(DamageArgument damage)
    {
        Strenght -= damage.Damage;
    }

    public bool HasFood()
    {
        return food.BaseValue > 0;
    }

    public void EatFood()
    {
        Contract.Assert(HasFood());

        food.BaseValue -= 1;
        strenght.BaseValue = Math.Max(start_strenght.BaseValue, strenght.BaseValue + 4);
    }

    #region Potions
    public bool HasHabilityPotion()
    {
        return potion_ability.BaseValue > 0;
    }

    public void DrinkHabilityPotion()
    {
        Contract.Assert(HasHabilityPotion());

        potion_ability.BaseValue -= 1;
        ability.BaseValue = start_ability.BaseValue;
    }

    public bool HasStrenghtPotion()
    {
        return potion_strenght.BaseValue > 0;
    }

    public void DrinkStrenghtPotion()
    {
        Contract.Assert(HasStrenghtPotion());

        potion_strenght.BaseValue -= 1;
        strenght.BaseValue = start_strenght.BaseValue;
    }

    public bool HasLuckPotion()
    {
        return potion_luck.BaseValue > 0;
    }

    public void DrinkLuckPotion()
    {
        Contract.Assert(HasLuckPotion());

        potion_luck.BaseValue -= 1;
        start_luck.BaseValue += 1;
        luck.BaseValue = start_luck.BaseValue;
    }

    public DamageArgument Attack(Agent agent)
    {
        // To work, should not be called by ennemies, if they initiate attacks, use Defend instead.
        Debug.Assert(IsHero);

        int heroAssault = Roll.RollMacro("2d6") + Ability;
        int foeAssault = Roll.RollMacro("2d6") + agent.Ability;
        if(heroAssault > foeAssault)
        {
            return new DamageArgument() {
                Dealer = this,
                Target = agent,
                Damage = 2
            };
        } else {
            return new DamageArgument() {
                Dealer = agent,
                Target = this,
                Damage = 2
            };
        }
    }

    public DamageArgument Defend(Agent agent)
    {
        // To work, should not be called by ennemies.
        Debug.Assert(IsHero);

        int heroAssault = Roll.RollMacro("2d6") + Ability;
        int foeAssault = Roll.RollMacro("2d6") + agent.Ability;
        if(heroAssault > foeAssault)
        {
            return new DamageArgument() {
                Dealer = agent,
                Target = this,
                Damage = 0
            };
        } else {
            return new DamageArgument() {
                Dealer = agent,
                Target = this,
                Damage = 2
            };
        }
    }
    #endregion

}