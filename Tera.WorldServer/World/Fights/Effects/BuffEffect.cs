using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Enumerations;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.World.Fights.Effects
{
    public enum BuffActiveType
    {
        ACTIVE_STATS,
        ACTIVE_BEGINTURN,
        ACTIVE_ENDTURN,
        ACTIVE_ENDMOVE,
        ACTIVE_ATTACK_POST_JET,
        ACTIVE_ATTACK_AFTER_JET,
        ACTIVE_ATTACKED_POST_JET,
        ACTIVE_ATTACKED_AFTER_JET,
    }

    public enum BuffDecrementType
    {
        TYPE_BEGINTURN,
        TYPE_ENDTURN,
        TYPE_ENDMOVE,
    }

    public abstract class BuffEffect
    {
        public BuffDecrementType DecrementType
        {
            get;
            set;
        }

        public BuffActiveType ActiveType
        {
            get;
            set;
        }
        
        public EffectCast CastInfos
        {
            get;
            set;
        }
                 
        public Fighter Caster
        {
            get;
            set;
        }

        public Fighter Target
        {
            get;
            set;
        }

        public int Duration
        {
            get;
            set;
        }

        public bool IsDebuffable
        {
            get
            {
                switch (this.CastInfos.EffectType)
                {
                    case EffectEnum.Porter:
                    case EffectEnum.AddState:
                    case EffectEnum.ChangeSkin:
                    case EffectEnum.AddChatiment:
                    case EffectEnum.AddPA:
                    case EffectEnum.AddPABis:
                    case EffectEnum.AddPM:
                    case EffectEnum.SubPA:
                    case EffectEnum.SubPM:
                    case EffectEnum.SubPAEsquivable:
                    case EffectEnum.SubPMEsquivable:
                    case EffectEnum.CasterSubPA:
                    case EffectEnum.CasterAddPO:
                    case EffectEnum.CasterSubPO:
                    case EffectEnum.CasterSubPM:
                    case EffectEnum.IncreaseSpellDamage:
                        return false;
                }

                return true;
            }
        }
        
        public BuffEffect(EffectCast CastInfos, Fighter Target, BuffActiveType ActiveType, BuffDecrementType DecrementType)
        {
            this.CastInfos = CastInfos;
            this.Duration = Target.Fight.CurrentFighter == Target ? CastInfos.Duration + 1 : CastInfos.Duration;
            this.Caster = CastInfos.Caster;
            this.Target = Target;

            this.ActiveType = ActiveType;
            this.DecrementType = DecrementType;
            
            switch(CastInfos.EffectType)
            {
                case EffectEnum.ReflectSpell:
                    Target.Fight.SendToFight(new GameEffectInformationsMessage(this.CastInfos.EffectType,
                                                                               this.Target.ActorId,
                                                                               "-1",
                                                                               this.CastInfos.Value2.ToString(),
                                                                               "10",
                                                                               "",
                                                                               this.CastInfos.Duration.ToString(),
                                                                               this.CastInfos.SpellId.ToString()));
                    break;

                case EffectEnum.ChanceEcaflip:
                case EffectEnum.AddChatiment:
                    Target.Fight.SendToFight(new GameEffectInformationsMessage(this.CastInfos.EffectType,
                                                                           this.Target.ActorId,
                                                                           this.CastInfos.Value1.ToString(),
                                                                           this.CastInfos.Value2.ToString(),
                                                                           this.CastInfos.Value3.ToString(),
                                                                           "",
                                                                           this.CastInfos.Duration.ToString(),
                                                                           this.CastInfos.SpellId.ToString()));
                    break;

                case EffectEnum.Porter:
                    Target.Fight.SendToFight(new GameEffectInformationsMessage(this.CastInfos.EffectType,
                                                                          this.Caster.ActorId,
                                                                          this.CastInfos.Value1.ToString(),
                                                                          "",
                                                                          "",
                                                                          "",
                                                                          this.CastInfos.Duration.ToString(),
                                                                          this.CastInfos.SpellId.ToString()));
                    break;

                default:
                    Target.Fight.SendToFight(new GameEffectInformationsMessage(this.CastInfos.EffectType,
                                                                               this.Target.ActorId,
                                                                               this.CastInfos.Value1.ToString(),
                                                                               "",
                                                                               "",
                                                                               "",
                                                                               this.CastInfos.Duration.ToString(),
                                                                               this.CastInfos.SpellId.ToString()));
                    break;
            }
        }

        public virtual int ApplyEffect(ref int DamageValue, EffectCast DamageInfos = null)
        {
            return this.Target.TryDie(this.Caster.ActorId);
        }

        /// <summary>
        /// Fin du buff
        /// </summary>
        /// <returns></returns>
        public virtual int RemoveEffect()
        {
            return this.Target.TryDie(this.Caster.ActorId);
        }

        /// <summary>
        /// Decrement le buff
        /// </summary>
        public int DecrementDuration()
        {
            this.Duration--;

            this.CastInfos.FakeValue = 0;

            return this.Duration;            
        }
    }
}
