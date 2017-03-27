using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.WorldServer.World.Fights;

namespace Tera.WorldServer.World.Challenges
{
    public sealed class Zombie : Challenge
    {
        public Zombie(Fight Fight)
            : base(1, Fight)
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
            if (Fighter.Team == Fight.Team1 && Path.MovementLength != 1)
                Failure(Fighter);
        }

        public override void onBeginTurn(Fighter newFighter)
        {
        }

        public override void onMiddleTurn(Fighter fighter)
        {
        }

        public override void onEndTurn(Fighter endFighter, bool Finish = false)
        {
            if (endFighter.Team == Fight.Team1 && endFighter.UsedMP != 1)
                Failure(endFighter);
        }

        public override void onDie(Fighter fighterDead, Fighter Caster)
        { 
        }

    }
}
