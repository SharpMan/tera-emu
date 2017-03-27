using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Enumerations;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.World.Character;
using Tera.WorldServer.World.Packets;
using Tera.WorldServer.World.Fights.FightObjects;

namespace Tera.WorldServer.World.Fights.Effects
{
    public sealed class EffectInvocationDouble : EffectBase
    {
        public override int ApplyEffect(EffectCast CastInfos)
        {
            // Possibilité de spawn une creature sur la case ?
            if (CastInfos.Caster.Fight.IsCellWalkable(CastInfos.CellId))
            {
                var ActorID = getNextLowerFighterGuid(CastInfos.Caster.Fight);
                var Character = ClonePerso(((CharacterFighter)CastInfos.Caster).Character, ActorID);

                // Template de monstre existante
                if (Character != null)
                {
                    // Level de monstre existant
                        var Double = new DoubleFighter(CastInfos.Caster.Fight, Character,CastInfos.Caster);
                        Double.Fight.JoinFightTeam(Double, CastInfos.Caster.Team, false, CastInfos.CellId);
                        Double.Fight.RemakeTurns();
                        Double.Fight.SendToFight(new GameInformationCoordinateMessage(Double.Fight.Fighters));
                        Double.Fight.SendToFight(new GameTurnListMessage(Double.Fight.getWorkerFighters()));
                        Double.Fight.GetCell(CastInfos.CellId).GetObjects<FightGroundLayer>().ForEach(x => x.onWalkOnLayer(Double, Double.Fight.GetCell(CastInfos.CellId)));
                }
            }

            return -1;
        }

        public long getNextLowerFighterGuid(Fight fight)
        {
            long g = -1;
            foreach (Fighter f in fight.Fighters)
            {
                if (f.ActorId < g)
                {
                    g = f.ActorId;
                }
            }
            g--;
            return g;
        }

        public static Player ClonePerso(Player perso, long id)
        {
            var Character = new Player()
            {
                ID = id,
                Name = perso.Name,
                Level = perso.Level,
                Color1 = perso.Color1,
                Color2 = perso.Color2,
                Color3 = perso.Color3,
                Look = perso.Look,
                Sexe = perso.Sexe,
                Classe = perso.Classe,
                Alignement = perso.Alignement,
                Honor = perso.Honor,
                Deshonor = perso.Deshonor,
                showWings = perso.showWings,
                InventoryCache = perso.InventoryCache,
                Mount = perso.Mount,
                onMount = perso.onMount,
                Life = perso.Life,
                mySpells = perso.mySpells,
                myMap = perso.myMap,
                Map = perso.Map,
                CellId = perso.CellId,
            };
            Character.myStats = new GenericStats(Character);
            Character.myStats.AddBase(EffectEnum.AddVitalite, perso.myStats.GetEffect(EffectEnum.AddVitalite).Base);
            Character.myStats.AddBase(EffectEnum.AddForce, perso.myStats.GetEffect(EffectEnum.AddForce).Base);
            Character.myStats.AddBase(EffectEnum.AddSagesse, perso.myStats.GetEffect(EffectEnum.AddSagesse).Base);
            Character.myStats.AddBase(EffectEnum.AddIntelligence, perso.myStats.GetEffect(EffectEnum.AddIntelligence).Base);
            Character.myStats.AddBase(EffectEnum.AddChance, perso.myStats.GetEffect(EffectEnum.AddChance).Base);
            Character.myStats.AddBase(EffectEnum.AddAgilite, perso.myStats.GetEffect(EffectEnum.AddAgilite).Base);
            Character.myStats.AddBase(EffectEnum.AddPA, perso.myStats.GetEffect(EffectEnum.AddPA).Base);
            Character.myStats.AddBase(EffectEnum.AddPM, perso.myStats.GetEffect(EffectEnum.AddPM).Base);
            Character.myStats.AddBase(EffectEnum.AddReduceDamagePourcentNeutre, perso.myStats.GetEffect(EffectEnum.AddReduceDamagePourcentNeutre).Base);
            Character.myStats.AddBase(EffectEnum.AddReduceDamagePourcentTerre, perso.myStats.GetEffect(EffectEnum.AddReduceDamagePourcentTerre).Base);
            Character.myStats.AddBase(EffectEnum.AddReduceDamagePourcentFeu, perso.myStats.GetEffect(EffectEnum.AddReduceDamagePourcentFeu).Base);
            Character.myStats.AddBase(EffectEnum.AddReduceDamagePourcentEau, perso.myStats.GetEffect(EffectEnum.AddReduceDamagePourcentEau).Base);
            Character.myStats.AddBase(EffectEnum.AddReduceDamagePourcentAir, perso.myStats.GetEffect(EffectEnum.AddReduceDamagePourcentAir).Base);
            Character.myStats.AddBase(EffectEnum.AddEsquivePA, perso.myStats.GetEffect(EffectEnum.AddEsquivePA).Base);
            Character.myStats.AddBase(EffectEnum.AddEsquivePM, perso.myStats.GetEffect(EffectEnum.AddEsquivePM).Base);
            return Character;
        }

    }
}
