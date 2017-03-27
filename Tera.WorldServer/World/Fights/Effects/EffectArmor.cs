using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Enumerations;

namespace Tera.WorldServer.World.Fights.Effects
{
    public sealed class EffectArmor : EffectBase
    { 
        public override int ApplyEffect(EffectCast CastInfos)
        {            
            foreach(var Target in CastInfos.Targets)
            {
                switch (CastInfos.SpellId)
                {
                    case 1:
                        if (Target.Team != CastInfos.Caster.Team)
                            continue;
                        Target.Stats.AddBoost(EffectEnum.AddArmorFeu, CastInfos.Value1);
                        break;

                    case 6:
                        if (Target.Team != CastInfos.Caster.Team)
                            continue;
                        Target.Stats.AddBoost(EffectEnum.AddArmorTerre, CastInfos.Value1);
                        break;

                    case 14:
                        if (Target.Team != CastInfos.Caster.Team)
                            continue;
                        Target.Stats.AddBoost(EffectEnum.AddArmorAir, CastInfos.Value1);
                        break;

                    case 18:
                        if (Target.Team != CastInfos.Caster.Team)
                            continue;
                        Target.Stats.AddBoost(EffectEnum.AddArmorEau, CastInfos.Value1);
                        break;

                    default:
                        Target.Stats.AddBoost(EffectEnum.AddArmor, CastInfos.Value1);
                        break;
                }
                
                // Ajout du buff
                Target.Buffs.AddBuff(new BuffArmor(CastInfos, Target));
            }

            return -1;
        }
    }
}
