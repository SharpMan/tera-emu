using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Enumerations;
using Tera.WorldServer.World.Maps;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.World.Fights.Effects
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class EffectLancer : EffectBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="CastInfos"></param>
        /// <returns></returns>
        public override int ApplyEffect(EffectCast CastInfos)
        {
            var Infos = CastInfos.Caster.States.FindState(FighterStateEnum.STATE_PORTEUR);

            if (Infos != null)
            {
                var Target = Infos.Target;

                if (Target.States.HasState(FighterStateEnum.STATE_PORTE))
                {
                    var SleepTime = 1 + (200 * Pathfinder.GoalDistance(Target.Fight.Map, Target.CellId, CastInfos.CellId));

                    Target.Fight.SendToFight(new GameActionMessage((int)EffectEnum.Lancer, CastInfos.Caster.ActorId, CastInfos.CellId.ToString()));

                    System.Threading.Thread.Sleep(SleepTime);

                    return Target.SetCell(Target.Fight.GetCell(CastInfos.CellId));
                }
            }

            return -1;
        }
    }
}
