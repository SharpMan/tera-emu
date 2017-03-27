using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tera.WorldServer.World.Fights.Effects
{
    /// <summary>
    /// Classe de gestion des vols de vie
    /// </summary>
    public sealed class EffectLifeSteal : EffectBase
    {
        public override int ApplyEffect(EffectCast CastInfos)
        {
            foreach (var Target in CastInfos.Targets)
            {
                var DamageJet = CastInfos.GenerateJet(Target);

                if (EffectDamage.ApplyDamages(CastInfos, Target, ref DamageJet) == -3)
                    return -3;

                var HealJet = DamageJet / 2;

                if (EffectHeal.ApplyHeal(CastInfos, CastInfos.Caster, HealJet) == -3)
                    return -3;                
            }

            return -1;
        }
    }
}
