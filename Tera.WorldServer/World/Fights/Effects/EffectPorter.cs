using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tera.WorldServer.World.Fights.Effects
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class EffectPorter : EffectBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="CastInfos"></param>
        /// <returns></returns>
        public override int ApplyEffect(EffectCast CastInfos)
        {
            foreach (var Target in CastInfos.Targets)
            {
                if (Target.ObjectType == FightObjectType.OBJECT_STATIC)
                {
                    continue;
                }
                var PorteurInfos = new EffectCast(CastInfos.EffectType, CastInfos.SpellId, 0, 0, 0, (int)FighterStateEnum.STATE_PORTEUR, 0, 0, CastInfos.Caster, null);
                var PorterInfos = new EffectCast(CastInfos.EffectType, CastInfos.SpellId, 0, 0, 0, (int)FighterStateEnum.STATE_PORTE, 0, 0, CastInfos.Caster, null);

                CastInfos.Caster.Buffs.AddBuff(new BuffPorteur(PorteurInfos, Target));
                Target.Buffs.AddBuff(new BuffPorter(PorterInfos, Target));                
            }

            return -1;
        }
    }
}
