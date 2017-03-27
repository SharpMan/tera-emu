using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;

namespace Tera.WorldServer.World.Fights.Effects
{
    public class EffectLifeDamage : EffectBase
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
                    Target.Buffs.AddBuff(new BuffLifeDamage(CastInfos, Target));
                }
            }
            else // Dommage direct
            {
                foreach (var Target in CastInfos.Targets)
                {
                    int effectBase = CastInfos.GenerateJet(Target);
                    var DamageValue = (Target.CurrentLife / 100) * effectBase;
                    //DamageValue = (-DamageValue);
                    if (EffectDamage.ApplyDamages(CastInfos, Target, ref DamageValue) == -3)
                        return -3;
                }
            }

            return -1;
        }
    }
}
