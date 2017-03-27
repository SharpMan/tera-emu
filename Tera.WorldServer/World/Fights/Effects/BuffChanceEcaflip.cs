using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tera.WorldServer.World.Fights.Effects
{
    public sealed class BuffChanceEcaflip : BuffEffect
    {
        private static Random Random = new Random();

        public BuffChanceEcaflip(EffectCast CastInfos, Fighter Target)
            : base(CastInfos, Target, BuffActiveType.ACTIVE_ATTACKED_AFTER_JET, BuffDecrementType.TYPE_ENDTURN)
        {
        }

        public override int ApplyEffect(ref int DamageValue, EffectCast DamageInfos = null)
        {
            var CoefDamages = this.CastInfos.Value1;
            var CoefHeal = this.CastInfos.Value2;
            var Chance = this.CastInfos.Value3;
            var Jet = BuffChanceEcaflip.Random.Next(0, 99);

            // Soin ?
            if (Jet < Chance)
            {
                var HealValue = DamageValue * CoefHeal;

                if (EffectHeal.ApplyHeal(this.CastInfos, this.Target, HealValue) == -3)
                    return -3;

                DamageValue = 0;
            }
            else // Multiplication des dommages
            {
                DamageValue *= CoefDamages;
            }

            return base.ApplyEffect(ref DamageValue, DamageInfos);
        }
    }
}
