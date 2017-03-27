using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tera.WorldServer.World.Fights.Effects
{
    public sealed class EffectSacrifice : EffectBase
    {
        public override int ApplyEffect(EffectCast CastInfos)
        {
            foreach (var Target in CastInfos.Targets)
            {
                if (Target.Team != CastInfos.Caster.Team || Target == CastInfos.Caster)
                    continue;

                Target.Buffs.AddBuff(new BuffSacrifice(CastInfos, Target));
            }

            return -1;
        }
    }
}
