using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tera.WorldServer.World.Fights.Effects
{
    public sealed class BuffDamageDropLife : BuffEffect
    {
        public BuffDamageDropLife(EffectCast CastInfos, Fighter Target)
            : base(CastInfos, Target, BuffActiveType.ACTIVE_BEGINTURN, BuffDecrementType.TYPE_ENDTURN)
        {

        }

        public override int ApplyEffect(ref int DamageJet, EffectCast DamageInfos = null)
        {
            //var Damage = this.CastInfos.RandomJet;

            // return EffectDamage.ApplyDamages(this.CastInfos, this.Target, ref Damage);


                int effectBase = DamageJet;
                var DamageValuea = (Target.CurrentLife / 100) * effectBase;
                var DamageValue = (CastInfos.Caster.CurrentLife / 100) * effectBase;
                if (EffectDamage.ApplyDamages(CastInfos, CastInfos.Caster, ref DamageValue) == -3)
                {
                    EffectHeal.ApplyHeal(CastInfos, Target, DamageValue);
                    return -3;
                }
                else 
                    return EffectHeal.ApplyHeal(CastInfos, Target, DamageValue);
            
            //DamageValuea = (-DamageValuea);
        }
    }
}
