using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Enumerations;

namespace Tera.WorldServer.World.Fights.Effects
{
    public sealed class EffectStatsSteal : EffectBase
    {
        public static Dictionary<EffectEnum, EffectEnum> TargetMalus = new Dictionary<EffectEnum, EffectEnum>()
        {
            { EffectEnum.VolForce           , EffectEnum.SubForce         },
            { EffectEnum.VolIntell          , EffectEnum.SubIntelligence  },
            { EffectEnum.VolAgi             , EffectEnum.SubAgilite       },
            { EffectEnum.VolSagesse         , EffectEnum.SubSagesse       },
            { EffectEnum.VolChance          , EffectEnum.SubChance        },
            { EffectEnum.VolPA              , EffectEnum.SubPA            },
            { EffectEnum.VolPM              , EffectEnum.SubPM            },
            { EffectEnum.VolPO              , EffectEnum.SubPO            },
        };

        public static Dictionary<EffectEnum, EffectEnum> CasterBonus = new Dictionary<EffectEnum, EffectEnum>()
        {
            { EffectEnum.VolForce           , EffectEnum.AddForce         },
            { EffectEnum.VolIntell          , EffectEnum.AddIntelligence  },
            { EffectEnum.VolAgi             , EffectEnum.AddAgilite       },
            { EffectEnum.VolSagesse         , EffectEnum.AddSagesse       },
            { EffectEnum.VolChance          , EffectEnum.AddChance        },
            { EffectEnum.VolPA              , EffectEnum.AddPA            },
            { EffectEnum.VolPM              , EffectEnum.AddPM            },
            { EffectEnum.VolPO              , EffectEnum.AddPO            },
        };

        public override int ApplyEffect(EffectCast CastInfos)
        {
            var MalusType = TargetMalus[CastInfos.EffectType];
            var BonusType = CasterBonus[CastInfos.EffectType];

            var MalusInfos = new EffectCast(MalusType, CastInfos.SpellId, CastInfos.CellId, CastInfos.Value1, CastInfos.Value2, CastInfos.Value3, CastInfos.Chance, CastInfos.Duration, CastInfos.Caster, CastInfos.Targets);
            var BonusInfos = new EffectCast(BonusType, CastInfos.SpellId, CastInfos.CellId, CastInfos.Value1, CastInfos.Value2, CastInfos.Value3, CastInfos.Chance, CastInfos.Duration - 1, CastInfos.Caster, CastInfos.Targets);
            var DamageValue = 0;

            foreach (var Target in CastInfos.Targets)
            {
                if (Target == CastInfos.Caster)
                    continue;
                        
                // Malus a la cible
                var BuffStats = new BuffStats(MalusInfos, Target);            
                if (BuffStats.ApplyEffect(ref DamageValue) == -3)
                    return -3;

                Target.Buffs.AddBuff(BuffStats);

                // Bonus au lanceur
                BuffStats = new BuffStats(BonusInfos, CastInfos.Caster);
                if (BuffStats.ApplyEffect(ref DamageValue) == -3)
                    return -3;

                CastInfos.Caster.Buffs.AddBuff(BuffStats);
            }

            return -1;
        }
    }
}
