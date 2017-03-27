using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tera.WorldServer.World.Fights.Effects
{
    public sealed class BuffDamage : BuffEffect
    {
        public BuffDamage(EffectCast CastInfos, Fighter Target)
            : base(CastInfos, Target, BuffActiveType.ACTIVE_BEGINTURN, BuffDecrementType.TYPE_ENDTURN)//BuffActiveType.ACTIVE_ENDTURN, BuffDecrementType.TYPE_ENDTURN
        {
        }

        public override int ApplyEffect(ref int DamageValue, EffectCast DamageInfos = null)
        {
            var Damage = this.CastInfos.GenerateJet(Target);

            return EffectDamage.ApplyDamages(this.CastInfos, this.Target, ref Damage);
        }
    }
}
