using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Enumerations;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.World.Fights.Effects
{
    public sealed class BuffSubPMEsquive : BuffEffect
    {
        public BuffSubPMEsquive(EffectCast CastInfos, Fighter Target)
            : base(CastInfos, Target, BuffActiveType.ACTIVE_STATS, BuffDecrementType.TYPE_ENDTURN)
        {
        }

        public override int ApplyEffect(ref int DamageValue, EffectCast DamageInfos = null)
        {
            var LostMP = CastInfos.Value1 > Target.AP ? Target.MP : CastInfos.Value1;
            CastInfos.Value1 = Target.CalculDodgeAPMP(CastInfos.Caster, LostMP, true);

            if (CastInfos.Value1 != LostMP)
            {
                Target.Fight.SendToFight(new GameActionMessage((int)GameActionTypeEnum.FIGHT_DODGE_SUBPM, Target.ActorId, Target.ActorId + "," + (LostMP - CastInfos.Value1)));
            }

            if (CastInfos.Value1 > 0)
            {
                var BuffStats = new BuffStats(new EffectCast(this.CastInfos.EffectType, this.CastInfos.SpellId, this.CastInfos.SpellId, CastInfos.Value1, 0, 0, 0, Duration, this.CastInfos.Caster, null), this.Target);
                BuffStats.ApplyEffect(ref LostMP);
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
