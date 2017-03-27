using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Enumerations;
using Tera.WorldServer.World.Fights;

namespace Tera.WorldServer.World.Challenges
{
    public sealed class PerduDeVue : Challenge
    {
        public PerduDeVue(Fight Fight) : base(23,Fight)
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
            foreach (var TargetEffect in TargetEffects)
            {
                if (TargetEffect.Key.EffectType == EffectEnum.SubPO || TargetEffect.Key.EffectType == EffectEnum.VolPO)
                {
                    if (TargetEffect.Value.Any(x => x.Team != Launcher.Team))
                    {
                        Failure(Launcher);
                        break;
                    }
                }
            }
        }

        public override void onLaunchSpell(Fighter Launcher, Spells.SpellLevel Spell, int TargetCellId, Fighter TargetFighter, Dictionary<Spells.EffectInfos, List<Fighter>> TargetEffects, bool IsCC, bool IsEchec)
        {
            if (Spell == null || Launcher.Team != Fight.Team1)
            {
                return;
            }
            foreach (var TargetEffect in TargetEffects)
            {
                if (TargetEffect.Key.EffectType == EffectEnum.SubPO || TargetEffect.Key.EffectType == EffectEnum.VolPO)
                {
                    if (TargetEffect.Value.Any(x => x.Team != Launcher.Team))
                    {
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
