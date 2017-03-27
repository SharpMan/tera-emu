using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Enumerations;

namespace Tera.WorldServer.World.Fights.Effects
{
    public sealed class BuffChatiment : BuffEffect
    {
        public BuffChatiment(EffectCast CastInfos, Fighter Target)
            : base(CastInfos, Target, BuffActiveType.ACTIVE_ATTACKED_AFTER_JET, BuffDecrementType.TYPE_ENDTURN)
        {
        }

        public override int ApplyEffect(ref int DamageValue, EffectCast DamageInfos = null)
        {
            var BuffValue = DamageValue / 2; // Divise par deux les stats a boost car c'est un personnage.
            var StatsType = (EffectEnum)this.CastInfos.Value1 == EffectEnum.Heal ? EffectEnum.AddVitalite : (EffectEnum)this.CastInfos.Value1;
            var MaxValue = this.CastInfos.Value2;
            var Duration = this.CastInfos.Value3;

            if (this.Target.Fight.CurrentFighter.ActorId == this.CastInfos.FakeValue)
            {
                if (this.CastInfos.DamageValue < MaxValue)
                {
                    if (this.CastInfos.DamageValue + BuffValue > MaxValue)
                    {
                        BuffValue = MaxValue - this.CastInfos.DamageValue;
                    }
                }
                else
                {
                    BuffValue = 0;
                }
            }
            else
            {
                this.CastInfos.DamageValue = 0;
                this.CastInfos.FakeValue = (int)this.Target.Fight.CurrentFighter.ActorId;

                if (this.CastInfos.DamageValue + BuffValue > MaxValue)
                {
                    BuffValue = MaxValue;
                }
            }

            if (BuffValue != 0)
            {
                this.CastInfos.DamageValue += BuffValue;

                var BuffStats = new BuffStats(new EffectCast(StatsType, this.CastInfos.SpellId, this.CastInfos.SpellId, BuffValue, 0, 0, 0, Duration, this.CastInfos.Caster, null), this.Target);
                BuffStats.ApplyEffect(ref BuffValue);
                this.Target.Buffs.AddBuff(BuffStats);
            }

            return base.ApplyEffect(ref DamageValue, DamageInfos);
        }

        public override int RemoveEffect()
        {
            return base.RemoveEffect();
        }
    }
}
