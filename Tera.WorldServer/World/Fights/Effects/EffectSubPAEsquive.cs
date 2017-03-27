using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Enumerations;

namespace Tera.WorldServer.World.Fights.Effects
{
    public sealed class EffectSubPAEsquive : EffectBase
    {
        public override int ApplyEffect(EffectCast CastInfos)
        {
            if (CastInfos.Duration > 1)
            {
                foreach (var Target in CastInfos.Targets)
                {
                    var SubInfos = new EffectCast(EffectEnum.SubPAEsquivable, CastInfos.SpellId, 0, CastInfos.Value1, 0, 0, 0, CastInfos.Duration, CastInfos.Caster, null);

                    var Buff = new BuffSubPAEsquive(SubInfos, Target);

                    Target.Buffs.AddBuff(Buff);
                }
            }
            else
            {
                var DamageValue = 0;
                foreach (var Target in CastInfos.Targets)
                {
                    var SubInfos = new EffectCast(EffectEnum.SubPAEsquivable, CastInfos.SpellId, 0, CastInfos.Value1, 0, 0, 0, 0, CastInfos.Caster, null);

                    var Buff = new BuffSubPAEsquive(SubInfos, Target);
                    Buff.ApplyEffect(ref DamageValue);

                    Target.Buffs.AddBuff(Buff);
                }
            }

            return -1;
        }
    }
}
