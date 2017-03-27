using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tera.WorldServer.World.Fights.Effects
{
    public sealed class BuffHeal : BuffEffect
    {
        public BuffHeal(EffectCast CastInfos, Fighter Target)
            : base(CastInfos, Target, BuffActiveType.ACTIVE_BEGINTURN, BuffDecrementType.TYPE_ENDTURN)
        {
        }

        public override int ApplyEffect(ref int DamageValue, EffectCast DamageInfos = null)
        {
            if (EffectHeal.ApplyHeal(CastInfos, Target, CastInfos.GenerateJet(Target)) == -3)
                return -3;
            return -1;
            //var Damage = this.CastInfos.RandomJet;

            //return EffectDamage.ApplyDamages(this.CastInfos, this.Target, ref Damage);
        }
    }
}
