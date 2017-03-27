using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Enumerations;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.World.Fights.Effects
{
    public sealed class EffectDebuff : EffectBase
    {
        public override int ApplyEffect(EffectCast CastInfos)
        {
            foreach (var Target in CastInfos.Targets)
            {
                if (Target.Buffs.Debuff() == -3)
                    return -3;

                Target.Fight.SendToFight(new GameActionMessage((int)EffectEnum.DeleteAllBonus, Target.ActorId, Target.ActorId.ToString()));
            }

            return -1;
        }
    }
}
