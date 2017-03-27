using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tera.WorldServer.World.Fights.Effects
{
    public sealed class EffectSkin : EffectBase
    {
        public override int ApplyEffect(EffectCast CastInfos)
        {
            if (CastInfos.Duration > 0)
            {
                foreach (var Target in CastInfos.Targets)
                {
                    Target.Buffs.AddBuff(new BuffSkin(CastInfos, Target));
                }
            }

            return -1;
        }
    }
}
