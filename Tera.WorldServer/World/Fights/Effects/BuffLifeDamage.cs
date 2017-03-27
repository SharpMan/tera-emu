using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tera.WorldServer.World.Fights.Effects
{
    public sealed class BuffLifeDamage : BuffEffect
    {
        public BuffLifeDamage(EffectCast CastInfos, Fighter Target)
            : base(CastInfos, Target, BuffActiveType.ACTIVE_BEGINTURN, BuffDecrementType.TYPE_ENDTURN)
        {
        }

        public override int ApplyEffect(ref int DamageValue, EffectCast DamageInfos = null)
        {
            //var Damage = this.CastInfos.RandomJet;

           // return EffectDamage.ApplyDamages(this.CastInfos, this.Target, ref Damage);

            int effectBase = CastInfos.GenerateJet(Target);
            var DamageValuea = (Target.CurrentLife / 100) * effectBase;
            //DamageValuea = (-DamageValuea);
            return EffectDamage.ApplyDamages(this.CastInfos, this.Target, ref DamageValuea);
        }
    }
}
