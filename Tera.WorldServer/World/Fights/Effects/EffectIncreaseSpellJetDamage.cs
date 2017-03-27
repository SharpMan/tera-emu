using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tera.WorldServer.World.Fights.Effects
{
    public sealed class EffectIncreaseSpellJetDamage : EffectBase 
    {
        public override int ApplyEffect(EffectCast CastInfos)
        {
            CastInfos.Caster.Buffs.AddBuff(new BuffIncreaseSpellJetDamage(CastInfos, CastInfos.Caster));

            return -1;
        }
    }
}
