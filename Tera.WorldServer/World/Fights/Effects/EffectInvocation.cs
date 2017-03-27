using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.WorldServer.Database.Tables;
using Tera.WorldServer.World.Packets;
using Tera.WorldServer.World.Fights.FightObjects;


namespace Tera.WorldServer.World.Fights.Effects
{
    /// <summary>
    /// Invocation d'une creature
    /// </summary>
    public sealed class EffectInvocation : EffectBase
    {
        public override int ApplyEffect(EffectCast CastInfos)
        {
            // Possibilité de spawn une creature sur la case ?
            if (CastInfos.Caster.Fight.IsCellWalkable(CastInfos.CellId))
            {
                var InvocationId = CastInfos.Value1;
                var InvocationLevel = CastInfos.Value2;
                var Monster = MonsterTable.GetMonster(InvocationId);
                
                // Template de monstre existante
                if (Monster != null)
                {
                    Monster.Initialize();
                    var MonsterLevel = Monster.GetLevelOrNear(InvocationLevel);

                    // Level de monstre existant
                    if (MonsterLevel != null)
                    {
                        var MonsteFighter = new MonsterFighter(CastInfos.Caster.Fight, MonsterLevel, CastInfos.Caster.Fight.NextActorInvocationId(CastInfos.Caster) , Invocator: CastInfos.Caster);
                        MonsteFighter.JoinFight();
                        MonsteFighter.Fight.JoinFightTeam(MonsteFighter, CastInfos.Caster.Team,false,CastInfos.CellId);
                        MonsteFighter.Fight.RemakeTurns();
                        MonsteFighter.Fight.SendToFight(new GameInformationCoordinateMessage(MonsteFighter.Fight.Fighters));
                        MonsteFighter.Fight.SendToFight(new GameTurnListMessage(MonsteFighter.Fight.getWorkerFighters()));
                        MonsteFighter.Fight.GetCell(CastInfos.CellId).GetObjects<FightGroundLayer>().ForEach(x => x.onWalkOnLayer(MonsteFighter, MonsteFighter.Fight.GetCell(CastInfos.CellId)));
                    }
                }
            }

            return -1;
        }
    }
}
