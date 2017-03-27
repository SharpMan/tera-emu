using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tera.WorldServer.World.Fights.Effects
{
    public sealed class BuffReflectSpell : BuffEffect
    {
        public int ReflectLevel = 0;

        public BuffReflectSpell(EffectCast CastInfos, Fighter Target)
            : base(CastInfos, Target, BuffActiveType.ACTIVE_STATS, BuffDecrementType.TYPE_ENDTURN)
        {
            this.ReflectLevel = CastInfos.SpellLevel.Level;
            this.Target.States.AddState(this);
        }

        public override int RemoveEffect()
        {
            this.Target.States.DelState(this);

            return base.RemoveEffect();
        }
    }
}
