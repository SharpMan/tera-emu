using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.WorldServer.World.Fights;
using Tera.WorldServer.World.Maps;

namespace Tera.WorldServer.World.Challenges
{
    public sealed class Hardi : Challenge
    {
        public Hardi(Fight Fight) : base(36,Fight)
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
            if (endFighter.Team != Fight.Team1)
            {
                return;
            }
            var Cells = Pathfinding.NearestCells(Fight.Map, endFighter.CellId);
            foreach (var player in Fight.GetEnnemyTeam(endFighter.Team).GetAliveFighters())
            {
                if (Cells.Contains(player.CellId))
                    return;
            }
            Failure(endFighter);
        }

        public override void onDie(Fighter fighterDead, Fighter Caster)
        {
        }
    }
}
