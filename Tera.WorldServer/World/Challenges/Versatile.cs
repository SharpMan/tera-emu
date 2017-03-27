using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.WorldServer.World.Fights;

namespace Tera.WorldServer.World.Challenges
{
    public class Versatile : Challenge
    {
        public Versatile(Fight Fight): base (6,Fight){
            BasicDropBonus = 10;
            BasicXpBonus = 10;

            TeamDropBonus = 15;
            TeamXpBonus = 15;

            ShowTarget = false;
            TargetId = 0;
        }

        private int lastAction = 0;

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
            if (lastAction == 0)
            {
                lastAction = -1;
            }
            else
            {
                if (lastAction != -1)
                {
                    Failure(Launcher);
                }
            }
        }

        public override void onLaunchSpell(Fighter Launcher, Spells.SpellLevel Spell, int TargetCellId, Fighter TargetFighter, Dictionary<Spells.EffectInfos, List<Fighter>> TargetEffects, bool IsCC, bool IsEchec)
        {
            if (Launcher.Team != Fight.Team1)
            {
                return;
            }
            if (lastAction == 0)
            {
                lastAction = Spell.SpellCache.ID;
            }
            else
            {
                if (lastAction != Spell.SpellCache.ID)
                {
                    Failure(Launcher);
                }
            }
        }

        public override void onActorMoved(Fighter Fighter, Maps.MovementPath Path, FightCell NewCell)
        {
        }

        public override void onBeginTurn(Fighter newFighter)
        {
            lastAction = 0;
        }

        public override void onMiddleTurn(Fighter fighter)
        {
        }

        public override void onEndTurn(Fighter endFighter, bool Finish = false)
        {
            lastAction = 0;
        }

        public override void onDie(Fighter fighterDead, Fighter Caster)
        {
        }
    }
}
