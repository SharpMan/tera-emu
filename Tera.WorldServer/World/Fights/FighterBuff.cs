using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.WorldServer.World.Fights.Effects;
using Tera.Libs.Enumerations;

namespace Tera.WorldServer.World.Fights
{
    public sealed class FighterBuff
    {
        private Dictionary<BuffActiveType, List<BuffEffect>> BuffsAct = new Dictionary<BuffActiveType, List<BuffEffect>>()
        {
            { BuffActiveType.ACTIVE_ATTACKED_AFTER_JET, new List<BuffEffect>() },
            { BuffActiveType.ACTIVE_ATTACKED_POST_JET, new List<BuffEffect>() },
            { BuffActiveType.ACTIVE_ATTACK_AFTER_JET, new List<BuffEffect>() },
            { BuffActiveType.ACTIVE_ATTACK_POST_JET, new List<BuffEffect>() },
            { BuffActiveType.ACTIVE_BEGINTURN, new List<BuffEffect>() },
            { BuffActiveType.ACTIVE_ENDTURN, new List<BuffEffect>() },
            { BuffActiveType.ACTIVE_ENDMOVE, new List<BuffEffect>() },
            { BuffActiveType.ACTIVE_STATS, new List<BuffEffect>() },
        };

        private Dictionary<BuffDecrementType, List<BuffEffect>> BuffsDec = new Dictionary<BuffDecrementType, List<BuffEffect>>()
        {
            { BuffDecrementType.TYPE_BEGINTURN, new List<BuffEffect>() },
            { BuffDecrementType.TYPE_ENDTURN, new List<BuffEffect>()   },
            { BuffDecrementType.TYPE_ENDMOVE, new List<BuffEffect>()    },
        };

        public List<BuffEffect> getBuffs()
        {
            List<BuffEffect> list = new List<BuffEffect>();
            foreach (List<BuffEffect> sublist in BuffsAct.Values)
            {
                list.AddRange(sublist);
            }
            return list;
        }

        public void AddBuff(BuffEffect Buff)
        {
            this.BuffsAct[Buff.ActiveType].Add(Buff);
            this.BuffsDec[Buff.DecrementType].Add(Buff);
        }

        public bool HasBuff(EffectEnum effect)
        {
            foreach (var BuffList in BuffsAct.Values)
            {
                if(BuffList.Exists(buff => buff.CastInfos.EffectType == effect)){
                    return true;
                }
            }
            return false;
        }

        public int BeginTurn()
        {
            var Damage = 0;
            foreach (var Buff in BuffsAct[BuffActiveType.ACTIVE_BEGINTURN])
                if (Buff.ApplyEffect(ref Damage) == -3)
                    return -3;

            foreach (var Buff in this.BuffsDec[BuffDecrementType.TYPE_BEGINTURN])
                if (Buff.DecrementDuration() <= 0)
                    if (Buff.RemoveEffect() == -3)
                        return -3;

            this.BuffsDec[BuffDecrementType.TYPE_BEGINTURN].RemoveAll(x => x.Duration <= 0);

            foreach (var BuffList in this.BuffsAct.Values)
                BuffList.RemoveAll(Buff => Buff.DecrementType == BuffDecrementType.TYPE_BEGINTURN && Buff.Duration <= 0);

            return -1;
        }

      
        public int EndTurn()
        {
            var Damage = 0;
            foreach (var Buff in BuffsAct[BuffActiveType.ACTIVE_ENDTURN])
                if (Buff.ApplyEffect(ref Damage) == -3)
                    return -3;

            foreach (var Buff in this.BuffsDec[BuffDecrementType.TYPE_ENDTURN])
                if (Buff.DecrementDuration() <= 0)
                    if (Buff.RemoveEffect() == -3)
                        return -3;

            this.BuffsDec[BuffDecrementType.TYPE_ENDTURN].RemoveAll(x => x.Duration <= 0);

            foreach (var BuffList in this.BuffsAct.Values)
                BuffList.RemoveAll(Buff => Buff.DecrementType == BuffDecrementType.TYPE_ENDTURN && Buff.Duration <= 0);

            return -1;
        }

        public int EndMove()
        {
            var Damage = 0;
            foreach (var Buff in BuffsAct[BuffActiveType.ACTIVE_ENDMOVE])
                if (Buff.ApplyEffect(ref Damage) == -3)
                    return -3;

            this.BuffsAct[BuffActiveType.ACTIVE_ENDMOVE].RemoveAll(x => x.DecrementType == BuffDecrementType.TYPE_ENDMOVE && x.Duration <= 0);

            return -1;
        }

        /// <summary>
        /// Lance une attaque, activation des buffs d'attaque avant le calcul du jet avec les statistiques
        /// </summary>
        /// <param name="CastInfos"></param>
        /// <param name="DamageValue"></param>
        public int OnAttackPostJet(EffectCast CastInfos, ref int DamageValue)
        {
            foreach (var Buff in BuffsAct[BuffActiveType.ACTIVE_ATTACK_POST_JET])
                if (Buff.ApplyEffect(ref DamageValue, CastInfos) == -3)
                    return -3;

            return -1;
        }

        /// <summary>
        /// Lance une attaque, activation des buffs d'attaque apres le calcul du jet avec les statistiques
        /// </summary>
        /// <param name="CastInfos"></param>
        /// <param name="DamageValue"></param>
        public int OnAttackAfterJet(EffectCast CastInfos, ref int DamageValue)
        {
            foreach (var Buff in BuffsAct[BuffActiveType.ACTIVE_ATTACK_AFTER_JET])
                if (Buff.ApplyEffect(ref DamageValue, CastInfos) == -3)
                    return -3;
            return -1;
        }

        /// <summary>
        /// Subit des dommages, activation des buffs de reduction, renvois, anihilation des dommages avant le calcul du jet
        /// </summary>
        /// <param name="CastInfos"></param>
        /// <param name="DamageValue"></param>
        public int OnAttackedPostJet(EffectCast CastInfos, ref int DamageValue)
        {
            foreach (var Buff in BuffsAct[BuffActiveType.ACTIVE_ATTACKED_POST_JET])
                if (Buff.ApplyEffect(ref DamageValue, CastInfos) == -3)
                    return -3;
            return -1;
        }

        /// <summary>
        /// Subit des dommages, activation des buffs de reduction, renvois, anihilation des dommages apres le calcul du jet
        /// </summary>
        /// <param name="CastInfos"></param>
        /// <param name="DamageValue"></param>
        public int OnAttackedAfterJet(EffectCast CastInfos, ref int DamageValue)
        {
            foreach (var Buff in BuffsAct[BuffActiveType.ACTIVE_ATTACKED_AFTER_JET])
                if (Buff.ApplyEffect(ref DamageValue, CastInfos) == -3)
                    return -3;
            return -1;
        }

        /// <summary>
        /// Debuff le personnage de tous les effets
        /// </summary>
        /// <returns></returns>
        public int Debuff()
        {
            foreach (var Buff in this.BuffsDec[BuffDecrementType.TYPE_BEGINTURN])
                if (Buff.IsDebuffable)
                    if (Buff.RemoveEffect() == -3)
                        return -3;

            foreach (var Buff in this.BuffsDec[BuffDecrementType.TYPE_ENDTURN])
                if (Buff.IsDebuffable)
                    if (Buff.RemoveEffect() == -3)
                        return -3;

            this.BuffsDec[BuffDecrementType.TYPE_BEGINTURN].RemoveAll(x => x.IsDebuffable);
            this.BuffsDec[BuffDecrementType.TYPE_ENDTURN].RemoveAll(x => x.IsDebuffable);

            foreach (var BuffList in this.BuffsAct.Values)
                BuffList.RemoveAll(x => x.IsDebuffable);

            return -1;
        }
    }
}
