using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using DocumentFormat.OpenXml.Drawing;
using Weaver.Tales;

namespace Storyder.FightingFantasySystem;

/// <summary>
/// How to manage combat if multiple opponents.
/// </summary>
public enum CombatType {
    /// <summary>
    /// The opponents are dealt one after the other.
    /// </summary>
    SUCCESSIVE,
    /// <summary>
    /// The opponents are dealt all at the same time,
    /// by only one can be attacked at each assault.
    /// </summary>
    TOGETHER
}

public class CombatEffect : StoryderEffect, ICommandArborescence, IStoryParagraphProvider
{
    public System System;

    public StoryderEffect Env { get; private set; }
    public StoryderEffect Victory { get; private set; }
    public StoryderEffect Escape { get; private set; }

    public int EscapeNbrAssault { get; private set; }
    public string EscapeDescription { get; private set; }
    public CombatType CombatType { get; private set; }
    public List<Agent> Ennemies { get; private set; }

    public int CurrentNbrAssaults;
    public List<Agent> CurrentEnnemies { get; private set; } = new();
    public StoryReader storyReader;

    public static CombatEffect Create(string[] args, System system)
    {
        CheckNumberArguments(args,3,3);

        // Ennemies
        string[] ennemiesStats = args[0].Split(',');
        List<Agent> ennemies = new();
        Debug.Assert(ennemiesStats.Length%3 == 0);
        int eindex = 0;
        while(eindex*3 < ennemiesStats.Length)
        {
            Agent e = new Agent("ennemi_"+eindex);
            e.Name = ennemiesStats[eindex*3+0].Trim();
            e.Ability = int.Parse(ennemiesStats[eindex*3+1].Trim());
            e.Stamina = int.Parse(ennemiesStats[eindex*3+2].Trim());
            ennemies.Add(e);
            eindex++;
        }

        // Escape
        string[] escapeArgs = args[1].Split(',');
        int nbrAssault = int.Parse(escapeArgs[0]);
        string escapeDesc = "";
        if(escapeArgs.Length > 1)
            escapeDesc = escapeArgs[1];

        // Combat Type
        CombatType cbType = CombatType.SUCCESSIVE;
        if(ennemies.Count>1)
            cbType = Enum.Parse<CombatType>(args[2]);
        
        
        CombatEffect ret = new() {
            System = system,
            Ennemies = ennemies,
            EscapeNbrAssault = nbrAssault,
            EscapeDescription = escapeDesc,
            CombatType = cbType
        };
        
        return ret;
    }

    public int SubNumber { get => 3; }

    public void AddSubEffects(StoryderEffect[] subs)
    {
        Debug.Assert(subs.Length == SubNumber);
        Env = subs[0];
        Victory = subs[1];
        Escape = subs[2];
    }

    public void GetSubEffects<T>(IList<T> subeffects)
    {
        if(Env is T g1)
            subeffects.Add(g1);
        if(Env is ICommandArborescence c1)
            c1.GetSubEffects<T>(subeffects);
        if(Victory is T g2)
            subeffects.Add(g2);
        if(Victory is ICommandArborescence c2)
            c2.GetSubEffects<T>(subeffects);
        if(Escape is T g3)
            subeffects.Add(g3);
        if(Escape is ICommandArborescence c3)
            c3.GetSubEffects<T>(subeffects);
    }

    public override void Actuate(StoryReader storyReader)
    {
        // Init working variables
        this.storyReader = storyReader;
        storyReader.DelayPostEffects(true);
        CurrentNbrAssaults = 0;
        CurrentEnnemies.Clear();
        foreach(Agent e in Ennemies)
            CurrentEnnemies.Add(new Agent(e.ModuleName) {
                Name = e.Name,
                Stamina = e.Stamina,
                Ability = e.Ability
            }
        );

        // Configure reader
        IStoryChoice _startCombat = new ComputedStoryChoice(this, "Commencer le combat");
        storyReader.AppendChoice(_startCombat);
    }

    public StoryParagraph GetNextStoryChoice(IStoryChoice choice)
    {
        StringBuilder  description = new();

        // Apply Damages from previous assault if any
        if(_assaultResults != null)
        {
            foreach(DamageArgument da in _assaultResults) {
                description.Append(da.ToParagraph()+"\n");
                da.ApplyDamage();
            }
            _assaultResults = null;
            description.AppendLine();
        }

        // Check for death
        if(System.Hero.IsDead) {
            // If player death, end paragraph
            description.Append("\n\n Vos blessures sont fatales, et la vie vous quitte peu a peu.");
            return new StoryParagraph() {
                Text = description.ToString()
            };
        }else{
            // Remove dead ennemies
            List<Agent> deads = new();
            foreach(Agent e in CurrentEnnemies) {
                if(e.IsDead) { deads.Add(e); }
            }
            foreach(Agent e in deads) {
                CurrentEnnemies.Remove(e);
                description.AppendFormat("{0} est mort !\n", e.Name);
            }
            description.AppendLine();
        }

        // Print belligerants status
        description.AppendFormat("[ Vous : Habileté {0} ; Endurance {1} ]\n",
            System.Hero.Ability, System.Hero.Stamina);
        foreach(Agent en in CurrentEnnemies)
        {
            description.AppendFormat("[ {2} : Habileté {0} ; Endurance {1} ]\n",
                en.Ability,
                en.Stamina,
                en.Name);
        }
        description.AppendLine();

        // Build new paragraph
        StoryParagraph next = new() {
            Text = description.ToString()
        };

        // Check for Victory
        if(CurrentEnnemies.Count == 0) {
            storyReader.DelayPostEffects(false);
            next.Text += "\tVictoire !!";
            next.Effects.Add(Victory);
            return next;
        }

        // Add environment effects
        if(Env != null)
            next.Effects.Add(Env);

        // Add attack choices
        List<IStoryChoice> _attack = new()
        {
            new ComputedStoryChoice(new Assault(this, CurrentEnnemies[0]),
            string.Format("[ Attack ennemy {0} ]", CurrentEnnemies[0].Name))
        };

        if(CurrentEnnemies.Count > 1 && CombatType == CombatType.TOGETHER) {
            foreach(Agent en in CurrentEnnemies) {
                if(en == CurrentEnnemies[0])
                    continue;
                _attack.Add(new ComputedStoryChoice(new Assault(this, en),
                    string.Format("[ Attack ennemy {0} ]", en.Name)));
            }
        }
        next.Choices.AddRange(_attack);

        // Add escape choice
        if(EscapeNbrAssault>=0 && EscapeNbrAssault<=CurrentNbrAssaults)
        {
            string escDesc = string.Format("[ Escape{0} ]",
                    string.IsNullOrWhiteSpace(EscapeDescription) ? "" : " : " + EscapeDescription);
            next.Choices.Add(new ComputedStoryChoice(new Escape(this), escDesc));
        }

        return next;
    }

    List<DamageArgument> _assaultResults;
    internal void SetAssaultResult(List<DamageArgument> assaultResults)
    {
        _assaultResults = assaultResults;
    }

    public override string GetTrace()
    {
        return "COMBAT";
    }
}

public class Escape : IStoryParagraphProvider
{
    private CombatEffect Combat;

    public Escape(CombatEffect combat)
    {
        Combat = combat;
    }

    public StoryParagraph GetNextStoryChoice(IStoryChoice choice)
    {
        Combat.System.Hero.Stamina -= 2;
        Combat.storyReader.DelayPostEffects(false);
        StoryParagraph next_escape =  new StoryParagraph() {
            Text = "Vous parvenez a vous enfuir, mais vos adversaires parviennent a vous blesser une derniere fois dans votre fuite.\nVous perdez 2 points d'Endurance.\n",
        };
        next_escape.Effects.Add(Combat.Escape);
        return next_escape;
    }
}

public class Assault : IStoryParagraphProvider
{
    private CombatEffect Combat;
    private Agent Target;

    public Assault(CombatEffect combat, Agent target)
    {
        Combat = combat;
        Target = target;
    }

    public StoryParagraph GetNextStoryChoice(IStoryChoice choice)
    {
        Combat.CurrentNbrAssaults++;
        StoryParagraph next = new() { };

        List<DamageArgument> _assaultResults = new()
        {
            Combat.System.Hero.Attack(Target)
        };
        if(Combat.CurrentEnnemies.Count > 1 && Combat.CombatType == CombatType.TOGETHER) {
            foreach(var enemy in Combat.CurrentEnnemies)
            {
                // Target assault already done
                if(enemy == Target)
                    continue;
                // Defend against other foes
                _assaultResults.Add(Combat.System.Hero.Defend(enemy));
            }
        }
        Combat.SetAssaultResult(_assaultResults);

        // Write Description
        StringBuilder desc = new();
        foreach(DamageArgument da in _assaultResults)
        {
            desc.AppendLine(da.ToParagraph());
        }

        next.Text = desc.ToString();
        next.Choices.Add(new ComputedStoryChoice(Combat, "Continuer"));

        foreach(DamageArgument da in _assaultResults
            .Where(x => x.Damage != 0)
            .Where(x => x.Damage < x.Target.Stamina))
        {
            next.Choices.Add(new ComputedStoryChoice(new UseLuck(Combat, da, _assaultResults), da.ToLuckChoice()));
        }

        return next;
    }
}

public class UseLuck : IStoryParagraphProvider
{
    public CombatEffect Combat { get; }
    public DamageArgument Damages { get; }
    private List<DamageArgument> _assaultResults;

    public UseLuck(CombatEffect combat, DamageArgument da, List<DamageArgument> assaultResults)
    {
        Combat = combat;
        Damages = da;
        _assaultResults = assaultResults;;
    }

    public StoryParagraph GetNextStoryChoice(IStoryChoice choice)
    {
        StoryParagraph next = new() { };

        if(Combat.System.Hero.TestLuckOnCombat(Damages))
            next.Text = "CHANCE réussie !\n\n";
        else
            next.Text = "CHANCE échouée !\n\n";
        next.Text += Damages.ToParagraph();
        
        next.Choices.Add(new ComputedStoryChoice(Combat) {
            Text = "Continuer",
        });

        foreach(DamageArgument da in _assaultResults
            .Where(x => !x.UsedLuck))
        {
            next.Choices.Add(new ComputedStoryChoice(new UseLuck(Combat, da, _assaultResults), da.ToLuckChoice()));
        }

        return next;
    }
}
