using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Enumerations;

namespace Tera.WorldServer.World.Fights.Effects
{
    public sealed class BuffArmor : BuffEffect
    {    
        public BuffArmor(EffectCast CastInfos, Fighter Target): base(CastInfos, Target, BuffActiveType.ACTIVE_ATTACKED_AFTER_JET, BuffDecrementType.TYPE_ENDTURN)
        {
        }
            
        public override int RemoveEffect()
        {
            switch (this.CastInfos.SpellId)
            {
                case 1:
                    this.Target.Stats.GetEffect(EffectEnum.AddArmorFeu).Boosts -= this.CastInfos.Value1;
                    break;

                case 6:
                    this.Target.Stats.GetEffect(EffectEnum.AddArmorNeutre).Boosts -= this.CastInfos.Value1;
                    this.Target.Stats.GetEffect(EffectEnum.AddArmorTerre).Boosts -= this.CastInfos.Value1;
                    break;

                case 14:
                    this.Target.Stats.GetEffect(EffectEnum.AddArmorAir).Boosts -= this.CastInfos.Value1;
                    break;

                case 18:
                    this.Target.Stats.GetEffect(EffectEnum.AddArmorEau).Boosts -= this.CastInfos.Value1;
                    break;

                default:
                    this.Target.Stats.GetEffect(EffectEnum.AddArmor).Boosts -= this.CastInfos.Value1;
                    break;
            }

            return base.RemoveEffect();
        }       
    }
}
