﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Enumerations;
using Tera.WorldServer.World.Fights;

namespace Tera.WorldServer.World.Challenges
{
    public sealed class Abnegation : Challenge
    {
        public Abnegation(Fight Fight) : base(43, Fight)
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
            foreach (var TargetEffect in TargetEffects)
            {
                if (TargetEffect.Key.EffectType == EffectEnum.AddVitalite || TargetEffect.Key.EffectType == EffectEnum.Heal)
                {
                    if(TargetEffect.Value.Any(x => x.Team == Launcher.Team)){
                        Failure(Launcher);
                        break;
                    }
                }
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
