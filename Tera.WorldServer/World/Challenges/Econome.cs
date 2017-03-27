using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.WorldServer.World.Fights;

namespace Tera.WorldServer.World.Challenges
{
    public sealed class Econome : Challenge
    {
        private Dictionary<Fighter, List<int>> UsedSpells = new Dictionary<Fighter, List<int>>();

        public Econome(Fight Fight)
            : base(5, Fight)
        {
            BasicDropBonus = 10;
            BasicXpBonus = 10;

            TeamDropBonus = 15;
            TeamXpBonus = 15;

            ShowTarget = false;
            TargetId = 0;
        }

        public override void onLeaveFight(Fighter Fighter)
        {
        }

        public override void onStartFight()
        {
        }

        public override void onLaunchWeapon(Fighter Launcher, Database.Models.InventoryItemModel Weapon, int TargetCellId, Fighter TargetFighter, Dictionary<Character.WeaponEffect, List<Fighter>> TargetEffects, bool IsCC, bool IsEchec, bool isPunch)
        {
            if (Launcher.Team != Fight.Team1)
            {
                return;
            }
            if (IsEchec)
            {
                return;
            }
            if (!UsedSpells.ContainsKey(Launcher))
            {
                UsedSpells.Add(Launcher, new List<int>());
            }
            if (UsedSpells[Launcher].Contains(-1))
            {
                Failure(Launcher);
            }
            else
            {
                UsedSpells[Launcher].Add(-1);
            }
        }

        public override void onLaunchSpell(Fighter Launcher, Spells.SpellLevel Spell, int TargetCellId, Fighter TargetFighter, Dictionary<Spells.EffectInfos, List<Fighter>> TargetEffects, bool IsCC, bool IsEchec)
        {
            if (Launcher.Team != Fight.Team1)
            {
                return;
            }
            if (IsEchec)
            {
                return;
            }
            if (!UsedSpells.ContainsKey(Launcher))
            {
                UsedSpells.Add(Launcher, new List<int>());
            }
            if (UsedSpells[Launcher].Contains(Spell.SpellCache.ID))
            {
                Failure(Launcher);
            }
            else
            {
                UsedSpells[Launcher].Add(Spell.SpellCache.ID);
            }
        }

        public override void onActorMoved(Fighter Fighter, Maps.MovementPath Path, FightCell NewCell)
        {
        }

        public override void onBeginTurn(Fighter newFighter)
        {
        }

        public override void onMiddleTurn(Fighter fighter)
        {
        }

        public override void onEndTurn(Fighter endFighter, bool Finish = false)
        {
        }

        public override void onDie(Fighter fighterDead, Fighter Caster)
        {
        }

    }
}
