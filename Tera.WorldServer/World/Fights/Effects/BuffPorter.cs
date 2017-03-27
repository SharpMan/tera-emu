using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tera.WorldServer.World.Fights.Effects
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class BuffPorter : BuffEffect
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="CastInfos"></param>
        /// <param name="Target"></param>
        public BuffPorter(EffectCast CastInfos, Fighter Target)
            : base(CastInfos, Target, BuffActiveType.ACTIVE_ENDMOVE, BuffDecrementType.TYPE_ENDMOVE)
        {
            this.Duration = int.MaxValue;

            this.Target.States.AddState(this);

            this.Target.SetCell(this.Caster.Cell);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="DamageValue"></param>
        /// <param name="DamageInfos"></param>
        /// <returns></returns>
        public override int ApplyEffect(ref int DamageValue, EffectCast DamageInfos = null)
        {
            if (this.Caster.CellId != this.Target.CellId)
            {
                this.Caster.States.RemoveState(FighterStateEnum.STATE_PORTEUR);
                this.Target.States.RemoveState(FighterStateEnum.STATE_PORTE);

                this.Duration = 0;
            }

            return -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int RemoveEffect()
        {
            this.Target.States.DelState(this);

            return base.RemoveEffect();
        }
    }
}
