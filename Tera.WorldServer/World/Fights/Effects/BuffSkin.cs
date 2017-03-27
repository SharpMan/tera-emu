using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Enumerations;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.World.Fights.Effects
{
    public sealed class BuffSkin : BuffEffect
    {
        public BuffSkin(EffectCast CastInfos, Fighter Target)
            : base(CastInfos, Target, BuffActiveType.ACTIVE_STATS, BuffDecrementType.TYPE_ENDTURN)
        {
            var DamageValue = 0;
            this.ApplyEffect(ref DamageValue);
        }

        public override int ApplyEffect(ref int DamageValue, EffectCast DamageInfos = null)
        {
            // On change le skin
            this.CastInfos.Value2 = this.Target.Skin;
            this.Target.Skin = this.CastInfos.Value3 == -1 ? this.Target.Skin : this.CastInfos.Value3;

            this.Target.Fight.SendToFight(new GameActionMessage((int)EffectEnum.ChangeSkin, this.Caster.ActorId, this.Target.ActorId + "," + this.CastInfos.Value2 + "," + this.Target.Skin + "," + this.Duration));

            return base.ApplyEffect(ref DamageValue, DamageInfos);
        }

        public override int RemoveEffect()
        {
            this.Target.Fight.SendToFight(new GameActionMessage((int)EffectEnum.ChangeSkin, this.Caster.ActorId, this.Target.ActorId + "," + this.Target.Skin + "," + this.CastInfos.Value2 + ",0"));

            this.Target.Skin = this.CastInfos.Value2;

            return base.RemoveEffect();
        }
    }
}
