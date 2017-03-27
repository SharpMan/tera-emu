using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Enumerations;

namespace Tera.WorldServer.World.Fights.Effects
{
    public sealed class BuffDamagePerPA : BuffEffect
    {
        public BuffDamagePerPA(EffectCast CastInfos, Fighter Target)
            : base(CastInfos, Target, BuffActiveType.ACTIVE_ENDTURN, BuffDecrementType.TYPE_ENDTURN)
        {

        }

        public override int ApplyEffect(ref int DamageValue, EffectCast DamageInfos = null)
        {
            int pas = CastInfos.Value1;
            int val = CastInfos.Value2;
            int nbr = (int)Math.Floor((double)Target.UsedAP / (double)pas);
            DamageValue = val * nbr;
            //Poison Paralysant
            if (CastInfos.SpellId == 200)
            {
                int inte = CastInfos.Caster.Stats.GetTotal(EffectEnum.AddIntelligence);
                if (inte < 0)
                {
                    inte = 0;
                }
                int pdom = CastInfos.Caster.Stats.GetTotal(EffectEnum.AddDamagePercent);
                if (pdom < 0)
                {
                    pdom = 0;
                }
                // on applique le boost
                // Ancienne formule : dgt = (int)(((100+inte+pdom)/100) *
                // dgt);
                DamageValue = (int)(((100 + inte + pdom) / 100) * DamageValue * 1.5);
            }

            return EffectDamage.ApplyDamages(this.CastInfos, this.Target, ref DamageValue);
        }
    }
}
