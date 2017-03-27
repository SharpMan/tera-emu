using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.World.Spells;
using Tera.WorldServer.World.Maps;
using Tera.WorldServer.World.Character;
using Tera.WorldServer.World.Fights.FightObjects;

namespace Tera.WorldServer.World.Fights
{
    public interface FightListener
    {
        /// <summary>
        /// Kické ou alors tout simplement annulé
        /// </summary>
        /// <param name="Character"></param>
        void onLeaveFight(Fighter Fighter);

        /// <summary>
        /// Fin du combat
        /// </summary>
        /// <param name="Winners"></param>
        /// <param name="Loosers"></param>
        void onEndFight(FightTeam Winners, FightTeam Loosers);

        /// <summary>
        /// Debut du combat
        /// </summary>
        /// <param name="Obj"></param>
        void onStartFight();

        /// <summary>
        /// Lancement du CAC
        /// </summary>
        /// <param name="Fighter"></param>
        /// <param name="InvetoryItem"></param>
        /// <param name="CellId"></param>
        void onLaunchWeapon(Fighter Launcher, InventoryItemModel Weapon, int TargetCellId, Fighter TargetFighter, Dictionary<WeaponEffect, List<Fighter>> TargetEffects, bool IsCC, bool IsEchec, bool isPunch);

        /// <summary>
        /// Lancement d'un sort
        /// </summary>
        /// <param name="Fighter"></param>
        /// <param name="SpellLevel"></param>
        /// <param name="CellId"></param>
        void onLaunchSpell(Fighter Launcher, SpellLevel Spell, int TargetCellId, Fighter TargetFighter, Dictionary<EffectInfos, List<Fighter>> TargetEffects, bool IsCC, bool IsEchec);

        /// <summary>
        /// Quand un GroundLayer est appliqué à un fighter
        /// </summary>
        void onApplyGroundLayer(FightGroundLayer layer, Fighter Caster, SpellLevel Spell, int TargetCellId, Fighter TargetFighter, Dictionary<EffectInfos, List<Fighter>> TargetEffects);

        /// <summary>
        /// Desabonne le client de la cell et l'ajoute sur la new
        /// </summary>
        /// <param name="Actor"></param>
        /// <param name="NewCell"></param>
        void onActorMoved(Fighter Fighter, MovementPath Path, FightCell NewCell);

        /// <summary>
        /// Debut du tour
        /// </summary>
        /// <param name="Obj"></param>
        void onBeginTurn(Fighter newFighter);

        /// <summary>
        /// A la fin d'un tour, reset etc
        /// </summary>
        void onMiddleTurn(Fighter fighter);

        /// <summary>
        /// Fin du tour
        /// </summary>
        /// <param name="Obj"></param>
        void onEndTurn(Fighter endFighter, bool Finish = false);

        /// <summary>
        /// Mort d'un personnage
        /// </summary>
        /// <param name="Obj"></param>
        void onDie(Fighter fighterDead, Fighter Caster);

    }
}
