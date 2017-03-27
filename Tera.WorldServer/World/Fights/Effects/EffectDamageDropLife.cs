using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tera.WorldServer.World.Fights.Effects
{
    public sealed class EffectDamageDropLife : EffectBase
    {
        public override int ApplyEffect(EffectCast CastInfos)
        {
            // Si > 0 alors c'est un buff
            if (CastInfos.Duration > 0)
            {
                // L'effet est un poison
                CastInfos.IsPoison = true;

                // Ajout du buff
                foreach (var Target in CastInfos.Targets)
                {
                    Target.Buffs.AddBuff(new BuffDamageDropLife(CastInfos, Target));
                }
            }
            else // Dommage direct
            {
                int effectBase = CastInfos.GenerateJet(CastInfos.Caster);
                var DamageValue = (CastInfos.Caster.CurrentLife / 100) * effectBase;
                if (EffectDamage.ApplyDamages(CastInfos, CastInfos.Caster, ref DamageValue) == -3)
                {
                    foreach (var Target in CastInfos.Targets)
                    {
                        if (EffectHeal.ApplyHeal(CastInfos, Target, DamageValue) == -3)
                            return -3;
                    }
                    return -3;
                }
                else
                {
                    foreach (var Target in CastInfos.Targets)
                    {
                        if (EffectHeal.ApplyHeal(CastInfos, Target, DamageValue) == -3)
                            return -3;
                    }
                }
                //DamageValue = (-DamageValue);

            }

            return -1;
        }
    }
}
