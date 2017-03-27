using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Enumerations;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.World.Fights.Effects
{
    public sealed class BuffSubPAEsquive : BuffEffect
    {
        public BuffSubPAEsquive(EffectCast CastInfos, Fighter Target)
            : base(CastInfos, Target, BuffActiveType.ACTIVE_STATS, BuffDecrementType.TYPE_ENDTURN)
        {
        }

        public override int ApplyEffect(ref int DamageValue, EffectCast DamageInfos = null)
        {
            var LostAP = CastInfos.Value1 > Target.AP ? Target.AP : CastInfos.Value1;
            CastInfos.Value1 = Target.CalculDodgeAPMP(CastInfos.Caster, LostAP);

            if (CastInfos.Value1 != LostAP)
            {
                Target.Fight.SendToFight(new GameActionMessage((int)GameActionTypeEnum.FIGHT_DODGE_SUBPA, Target.ActorId, Target.ActorId + "," + (LostAP - CastInfos.Value1)));
            }

            if (CastInfos.Value1 > 0)
            {
                var BuffStats = new BuffStats(new EffectCast(this.CastInfos.EffectType, this.CastInfos.SpellId, this.CastInfos.SpellId, CastInfos.Value1, 0, 0, 0, Duration, this.CastInfos.Caster, null), this.Target);
                BuffStats.ApplyEffect(ref LostAP);
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
