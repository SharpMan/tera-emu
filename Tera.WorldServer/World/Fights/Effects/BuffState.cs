using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tera.WorldServer.World.Fights.Effects
{
    public sealed class BuffState : BuffEffect
    {
        public BuffState(EffectCast CastInfos, Fighter Target)
            : base(CastInfos, Target, BuffActiveType.ACTIVE_STATS, BuffDecrementType.TYPE_ENDTURN)
        {
            var DamageValue = 0;
            this.ApplyEffect(ref DamageValue);
        }

        public override int ApplyEffect(ref int DamageValue, EffectCast DamageInfos = null)
        {
            this.Target.States.AddState(this);

            return base.ApplyEffect(ref DamageValue, DamageInfos);
        }

        public override int RemoveEffect()
        {
            this.Target.States.DelState(this);

            return base.RemoveEffect();
        }
    }
}
