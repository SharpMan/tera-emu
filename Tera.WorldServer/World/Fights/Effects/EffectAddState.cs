using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tera.WorldServer.World.Fights.Effects
{
    public sealed class EffectAddState : EffectBase
    {
        public override int ApplyEffect(EffectCast CastInfos)
        {
            foreach (var Target in CastInfos.Targets)
            {             
                Target.Buffs.AddBuff(new BuffState(CastInfos, Target));
            }

            return -1;
        }
    }
}
