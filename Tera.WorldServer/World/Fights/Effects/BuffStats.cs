using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Enumerations;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.World.Fights.Effects
{
    public sealed class BuffStats : BuffEffect
    {
        public BuffStats(EffectCast CastInfos, Fighter Target)
            : base(CastInfos, Target, BuffActiveType.ACTIVE_STATS, BuffDecrementType.TYPE_ENDTURN)
        {
        }

        public override int ApplyEffect(ref int DamageValue, EffectCast DamageInfos = null)
        {
            int ShowValue;

            switch (this.CastInfos.EffectType)
            {
                case EffectEnum.SubPA:
                case EffectEnum.SubPM:
                case EffectEnum.SubPAEsquivable:
                case EffectEnum.SubPMEsquivable:
                    ShowValue = -this.CastInfos.Value1;
                    break;

                default:
                    ShowValue = this.CastInfos.Value1;
                    break;
            }

            if(CastInfos.EffectType != EffectEnum.AddRenvoiDamage)
                this.Target.Fight.SendToFight(new GameActionMessage((int)this.CastInfos.EffectType, this.Target.ActorId, this.Target.ActorId + "," + ShowValue + "," + this.Duration));

            this.Target.Stats.AddBoost(this.CastInfos.EffectType, this.CastInfos.Value1);

            return base.ApplyEffect(ref DamageValue, DamageInfos);
        }

        public override int RemoveEffect()
        {
            this.Target.Stats.GetEffect(this.CastInfos.EffectType).Boosts -= this.CastInfos.Value1;

            return base.RemoveEffect();
        }
    }
}
