using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tera.WorldServer.World.Fights.Effects
{
    public sealed class BuffIncreaseSpellJetDamage : BuffEffect 
    {
        public BuffIncreaseSpellJetDamage(EffectCast CastInfos, Fighter Target)
            : base(CastInfos, Target, BuffActiveType.ACTIVE_ATTACK_POST_JET, BuffDecrementType.TYPE_ENDTURN)
        {
        }

        public override int ApplyEffect(ref int DamageValue, EffectCast DamageInfos = null)
        {
            if (DamageInfos.SpellId == this.CastInfos.SpellId)
                DamageValue += this.CastInfos.Value3;

            return base.ApplyEffect(ref DamageValue, DamageInfos);
        }
    }
}
