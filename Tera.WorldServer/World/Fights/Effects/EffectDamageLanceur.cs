using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;

namespace Tera.WorldServer.World.Fights.Effects
{
    public sealed class EffectDamageLanceur : EffectBase
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
                    Target.Buffs.AddBuff(new BuffDamage(CastInfos, CastInfos.Caster));
                }
            }
            else // Dommage direct
            {
                foreach (var Target in CastInfos.Targets)
                {
                    var DamageValue = CastInfos.GenerateJet(Target);
                    if (EffectDamage.ApplyDamages(CastInfos, CastInfos.Caster, ref DamageValue) == -3)
                        return -3;
                }
            }

            return -1;
        }

    }
}
