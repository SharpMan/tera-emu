using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Enumerations;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.World.Fights.Effects
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class BuffPorteur : BuffEffect
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="CastInfos"></param>
        /// <param name="Target"></param>
        public BuffPorteur(EffectCast CastInfos, Fighter Target)
            : base(CastInfos, Target, BuffActiveType.ACTIVE_ENDMOVE, BuffDecrementType.TYPE_ENDMOVE)
        {
            this.Duration = int.MaxValue;

            this.Caster.States.AddState(this);
           
            Target.Fight.SendToFight(new GameActionMessage((int)EffectEnum.Porter, CastInfos.Caster.ActorId, Target.ActorId.ToString()));          
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="DamageValue"></param>
        /// <param name="DamageInfos"></param>
        /// <returns></returns>
        public override int ApplyEffect(ref int DamageValue, EffectCast DamageInfos = null)
        {
            // Si effet finis
            if (!this.Target.States.HasState(FighterStateEnum.STATE_PORTE))
            {
                this.Duration = 0;
                return -1;
            }

            // On affecte la meme cell pour la cible porté
            return this.Target.SetCell(this.Caster.Cell);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int RemoveEffect()
        {
            this.Caster.States.DelState(this);

            return base.RemoveEffect();
        }
    }
}
