using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.Libs.Enumerations;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.World.Fights.Effects
{
    public sealed class EffectStats : EffectBase
    {
        public override int ApplyEffect(EffectCast CastInfos)
        {
            foreach (var Target in CastInfos.Targets)
            {
                var SubInfos = new EffectCast(CastInfos.EffectType, CastInfos.SpellId, CastInfos.CellId, CastInfos.GenerateJet(Target), CastInfos.Value2, CastInfos.Value3, CastInfos.Chance, CastInfos.Duration, CastInfos.Caster, CastInfos.Targets);
                var BuffStats = new BuffStats(SubInfos, Target);
                var DamageValue = 0;
                if (BuffStats.ApplyEffect(ref DamageValue) == -3)
                    return -3;

                Target.Buffs.AddBuff(BuffStats);
            }

            return -1;
        }
    }
}
