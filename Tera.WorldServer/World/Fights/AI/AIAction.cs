using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.WorldServer.World.Spells;
using Tera.Libs.Enumerations;
using Tera.Libs;

namespace Tera.WorldServer.World.Fights.AI
{
    public abstract class AIAction
    {
        public enum AIActionEnum
        {
            SELF_ACTING,//IA Automatique

            BUFF_HIMSELF,//Se buff en priorité
            BUFF_ALLY,///Buff les alliés en priorité

            SUBBUFF,//Ralenti les ennemis

            HEAL_HIMSELF,//Se soigne en priorité
            HEAL_ALLY,//Soigne les alliés en priorité

            ATTACK,//Attaque

            SUPPORT,//N'attaque pas mais soutient les alliés.

            DEBUFF_ALLY,//Debuff les alliés en priorité
            DEBUFF_ENNEMY,//Debuff les ennemis en priorité

            REPELS,//Repousse les ennemis.

            INVOK,//Invoque

            MAD,//Fou => Chaferfu
        }

        public static Dictionary<AIActionEnum, AIAction> AIActions = new Dictionary<AIActionEnum, AIAction>()
        {
            {AIActionEnum.SELF_ACTING, new SelfActingAction()},

            {AIActionEnum.BUFF_HIMSELF, new BuffHimselfAction()},
            {AIActionEnum.BUFF_ALLY, new BuffAllyAction()},

            {AIActionEnum.SUBBUFF, new SubBuffAction()},

            {AIActionEnum.HEAL_HIMSELF, new HealHimselfAction()},
            {AIActionEnum.HEAL_ALLY, new BuffAllyAction()},

            {AIActionEnum.ATTACK, new AttackAction()},

            {AIActionEnum.SUPPORT, new SupportAction()},

            {AIActionEnum.DEBUFF_ALLY, new DebuffAllyAction()},
            {AIActionEnum.DEBUFF_ENNEMY, new DebuffEnnemyAction()},

            {AIActionEnum.REPELS, new RepelsAction()},
            {AIActionEnum.INVOK, new InvokAction()},
            {AIActionEnum.MAD, new MadAction()},
        };

        protected abstract double ScoreHeal(AIProcessor AI, EffectInfos Effect, List<Fighter> Targets, bool Reverse = false);

        protected abstract double ScoreBuff_I(AIProcessor AI, EffectInfos Effect, List<Fighter> Targets, bool Reverse = false, bool notUseJet = false);
        protected abstract double ScoreBuff_II(AIProcessor AI, EffectInfos Effect, List<Fighter> Targets, bool Reverse = false, bool notUseJet = false);
        protected abstract double ScoreBuff_III(AIProcessor AI, EffectInfos Effect, List<Fighter> Targets, bool Reverse = false, bool notUseJet = false);

        protected abstract double ScoreDamage_0(AIProcessor AI, EffectInfos Effect, List<Fighter> Targets, bool Reverse = false);
        protected abstract double ScoreDamage_I(AIProcessor AI, EffectInfos Effect, List<Fighter> Targets, bool Reverse = false);
        protected abstract double ScoreDamage_II(AIProcessor AI, EffectInfos Effect, List<Fighter> Targets, bool Reverse = false);
        protected abstract double ScoreDamage_III(AIProcessor AI, EffectInfos Effect, List<Fighter> Targets, bool Reverse = false);
        
        protected abstract double ScoreDamagesPerPA(AIProcessor AI, EffectInfos Effect, List<Fighter> Targets, bool Reverse = false);
        protected abstract double ScoreSubBuff_I(AIProcessor AI, EffectInfos Effect, List<Fighter> Targets, bool Reverse = false, bool notUseJet = false);
        protected abstract double ScoreSubBuff_II(AIProcessor AI, EffectInfos Effect, List<Fighter> Targets, bool Reverse = false, bool notUseJet = false);
        protected abstract double ScoreSubBuff_III(AIProcessor AI, EffectInfos Effect, List<Fighter> Targets, bool Reverse = false, bool notUseJet = false);
        protected abstract double ScoreSubBuff_IV(AIProcessor AI, EffectInfos Effect, List<Fighter> Targets, bool Reverse = false, bool notUseJet = false);

        protected abstract double ScoreAddStateGood(AIProcessor AI, EffectInfos Effect, List<Fighter> Targets, bool Reverse = false);
        protected abstract double ScoreAddStateBad(AIProcessor AI, EffectInfos Effect, List<Fighter> Targets, bool Reverse = false);

        protected abstract double ScoreRemStateGood(AIProcessor AI, EffectInfos Effect, List<Fighter> Targets, bool Reverse = false);
        protected abstract double ScoreRemStateBad(AIProcessor AI, EffectInfos Effect, List<Fighter> Targets, bool Reverse = false);

        protected abstract double ScoreDebuff(AIProcessor AI, EffectInfos Effect, List<Fighter> Targets, bool Reverse = false);
        
        protected abstract double ScoreInvocation(AIProcessor AI, EffectInfos Effect, List<Fighter> Targets, bool Reverse = false, bool InvokPreview = false);
        protected abstract double ScoreInvocationStatic(AIProcessor AI, EffectInfos Effect, List<Fighter> Targets, bool Reverse = false, bool InvokPreview = false);

        protected abstract double ScoreRepulse(AIProcessor AI, int CastCell, EffectInfos Effect, List<Fighter> Targets, bool InvokPreview, bool isFear);
        protected abstract double ScoreAttract(AIProcessor AI, int CastCell, EffectInfos Effect, List<Fighter> Targets, bool InvokPreview);
        protected abstract double ScoreDeplace(AIProcessor AI, int CastCell, EffectInfos Effect, List<Fighter> Targets, bool InvokPreview, bool isThrow);
        protected abstract double ScoreExchangePlace(AIProcessor AI, int CasterCell, int CastCell, EffectInfos Effect, List<Fighter> Targets, bool InvokPreview);

        protected abstract double ScoreUseLayer(AIProcessor AI, int CastCell, EffectInfos Effect, List<Fighter> Targets, bool Reverse = false, bool notUseJet = false);

        public double GetEffectScore(AIProcessor AI, int CasterCell, int CastCell, EffectInfos Effect, List<Fighter> Targets, bool Reverse = false, bool InvokPreview = false)
        {
            switch (Effect.EffectType)
            {
                /* HEAL */
                case EffectEnum.RenderedPDV:
                case EffectEnum.Heal:
                case EffectEnum.DamageDropLife:
                    return ScoreHeal(AI, Effect,Targets, Reverse);

                /* BUFFS LEVEL 1*/
                case EffectEnum.ChanceEcaflip:
                    return Targets.Contains(AI.myFighter) ? (ScoreBuff_I(AI, Effect, Targets, Reverse)) : (-ScoreBuff_I(AI, Effect, Targets, Reverse));
                case EffectEnum.AddDamagePhysic:
                case EffectEnum.AddDamageMagic:
                case EffectEnum.AddDamagePercent:
                case EffectEnum.AddForce:
                case EffectEnum.AddIntelligence:
                case EffectEnum.AddChance:
                case EffectEnum.AddSagesse:
                case EffectEnum.AddAgilite:
                case EffectEnum.AddEsquivePA:
                case EffectEnum.AddEsquivePM:
                    return ScoreBuff_I(AI, Effect,Targets, Reverse);

                /* BUFFS LEVEL 2*/
                case EffectEnum.Invisible:
                    return ScoreBuff_II(AI, Effect,Targets, Reverse, true);
                case EffectEnum.BonusPM:
                case EffectEnum.AddPA:
                case EffectEnum.AddPABis:
                case EffectEnum.AddPO:
                case EffectEnum.AddPM:
                case EffectEnum.AddVitalite:
                case EffectEnum.DoubleDamageOrRendPDV:
                case EffectEnum.AddVie:
                case EffectEnum.AddDamage:
                case EffectEnum.AddCC:
                case EffectEnum.AddSoins:
                case EffectEnum.MultiplyDamage:
                case EffectEnum.AddReduceDamageMagic:
                case EffectEnum.AddReduceDamagePhysic:
                    return ScoreBuff_II(AI, Effect,Targets, Reverse);

                /* BUFFS LEVEL 3*/
                case EffectEnum.IncreaseSpellDamage:
                case EffectEnum.AddArmorBis:
                case EffectEnum.ReflectSpell:
                case EffectEnum.AddArmorNeutre:
                case EffectEnum.AddArmorTerre:
                case EffectEnum.AddArmorFeu:
                case EffectEnum.AddArmorEau:
                case EffectEnum.AddArmorAir:
                case EffectEnum.AddReduceDamagePourcentAir:
                case EffectEnum.AddReduceDamagePourcentEau:
                case EffectEnum.AddReduceDamagePourcentFeu:
                case EffectEnum.AddReduceDamagePourcentTerre:
                case EffectEnum.AddReduceDamagePourcentNeutre:
                case EffectEnum.AddReduceDamageAir:
                case EffectEnum.AddReduceDamageEau:
                case EffectEnum.AddReduceDamageFeu:
                case EffectEnum.AddReduceDamageTerre:
                case EffectEnum.AddReduceDamageNeutre:
                case EffectEnum.AddArmor:
                case EffectEnum.AddRenvoiDamage:
                case EffectEnum.Sacrifice:
                    return ScoreBuff_III(AI, Effect,Targets, Reverse);

                /* DAMAGE LEVEL 3*/
                case EffectEnum.VolEau:
                case EffectEnum.VolTerre: 
                case EffectEnum.VolAir:
                case EffectEnum.VolFeu:
                case EffectEnum.VolNeutre:
                case EffectEnum.Punition:
                    return ScoreDamage_III(AI, Effect,Targets, Reverse);

                /* DAMAGE LEVEL 2*/
                case EffectEnum.VolVie:
                case EffectEnum.DamageLifeAir:
                case EffectEnum.DamageLifeEau:
                case EffectEnum.DamageLifeFeu:
                case EffectEnum.DamageLifeNeutre:
                case EffectEnum.DamageLifeTerre:
                    return ScoreDamage_II(AI, Effect,Targets, Reverse);

                /* DAMAGE LEVEL 1*/
                case EffectEnum.DamageEau:
                case EffectEnum.DamageTerre:
                case EffectEnum.DamageAir:
                case EffectEnum.DamageFeu:
                case EffectEnum.DamageNeutre:
                    return ScoreDamage_I(AI, Effect,Targets, Reverse);

                /* DAMAGE LEVEL 0*/
                case EffectEnum.DamageLanceur:
                    return ScoreDamage_0(AI, Effect,Targets, Reverse);

                /* SUBBUFF LEVEL 4*/
                case EffectEnum.TurnPass:
                case EffectEnum.DieFighter:
                    return ScoreSubBuff_IV(AI, Effect,Targets, Reverse, true);

                /* SUBBUFF LEVEL 3*/
                case EffectEnum.VolPA:
                case EffectEnum.VolPO:
                case EffectEnum.VolPM:
                case EffectEnum.SubPA:
                case EffectEnum.SubPM:
                case EffectEnum.SubPO:
                case EffectEnum.SubDamage:
                case EffectEnum.SubVitalite:
                case EffectEnum.SubReduceDamagePourcentNeutre:
                case EffectEnum.SubReduceDamagePourcentFeu:
                case EffectEnum.SubReduceDamagePourcentEau:
                case EffectEnum.SubReduceDamagePourcentAir:
                case EffectEnum.SubReduceDamagePourcentTerre:
                    return ScoreSubBuff_III(AI, Effect,Targets, Reverse);

                /* SUBBUFF LEVEL 2*/
                case EffectEnum.DamagePerPA:
                    return ScoreDamagesPerPA(AI, Effect,Targets, Reverse);
                case EffectEnum.SubSoins:
                case EffectEnum.SubSagesse:
                case EffectEnum.SubForce:
                case EffectEnum.SubAgilite:
                case EffectEnum.SubChance:
                case EffectEnum.SubIntelligence:
                case EffectEnum.SubDamagePercent:
                case EffectEnum.SubDamagePhysic:
                case EffectEnum.SubDamageMagic:
                case EffectEnum.SubCC:
                case EffectEnum.AddEchecCritic:
                    return ScoreSubBuff_II(AI, Effect,Targets, Reverse);

                /* SUBBUFF LEVEL 1*/
                case EffectEnum.SubReduceDamageAir:
                case EffectEnum.SubReduceDamageEau:
                case EffectEnum.SubReduceDamageFeu:
                case EffectEnum.SubReduceDamageNeutre:
                case EffectEnum.SubReduceDamageTerre:
                case EffectEnum.SubPAEsquivable:
                case EffectEnum.SubPMEsquivable:
                    return ScoreSubBuff_I(AI, Effect,Targets, Reverse);

                /* ADDSTATE */
                case EffectEnum.AddState:
                    {
                        if (isGoodState(Effect))
                        {
                            return ScoreAddStateGood(AI, Effect,Targets, Reverse);
                        }
                        else
                        {
                            return ScoreAddStateBad(AI, Effect,Targets, Reverse);
                        }
                    }

                /* REMSTATE */
                case EffectEnum.LostState:
                    {
                        if (isGoodState(Effect))
                        {
                            return ScoreRemStateGood(AI, Effect,Targets, Reverse);
                        }
                        else
                        {
                            return ScoreRemStateBad(AI, Effect,Targets, Reverse);
                        }
                    }

                /* REPULSE */
                case EffectEnum.PushBack:
                    return ScoreRepulse(AI, CastCell, Effect, Targets, InvokPreview, false);
                case EffectEnum.PushFear:
                    return ScoreRepulse(AI, CastCell, Effect, Targets, InvokPreview, true);

                /* ATTRACT */
                case EffectEnum.PushFront:
                    return ScoreAttract(AI, CastCell, Effect, Targets, InvokPreview);

                /* DEPLACE */
                case EffectEnum.Porter:
                    return ScoreDeplace(AI, CastCell, Effect, Targets, InvokPreview, false);
                case EffectEnum.Lancer:
                    return ScoreDeplace(AI, CastCell, Effect, Targets, InvokPreview, true);

                /* EXCHANGE PLACE */
                case EffectEnum.Transpose:
                    return ScoreExchangePlace(AI, CasterCell, CastCell, Effect, Targets, InvokPreview);

                /* DEBUFF */
                case EffectEnum.DeleteAllBonus:
                    return ScoreDebuff(AI, Effect,Targets, Reverse);

                /* INVOCATION */
                case EffectEnum.InvocationStatic:
                    return ScoreInvocationStatic(AI, Effect, Targets, Reverse, InvokPreview);
                case EffectEnum.Invocation:
                    return ScoreInvocation(AI, Effect,Targets, Reverse, InvokPreview);
                case EffectEnum.DieFighterReplaceInvocation:
                    return ScoreSubBuff_IV(AI, Effect,Targets, Reverse, true) + ScoreInvocation(AI, Effect, Targets, Reverse, InvokPreview);

                case EffectEnum.UseGlyph:
                case EffectEnum.UseBlyph:
                case EffectEnum.UseTrap:
                    return ScoreUseLayer(AI, CastCell, Effect, Targets, Reverse);

                /* NADA */
                case EffectEnum.DoNothing:
                    return 0;

                default:
                    {
                        Logger.Info("Effect[" + Effect.EffectType + "] non defini pour l'IA");
                        return ScoreDamage_I(AI, Effect,Targets, Reverse);
                    }
            }
        }

        private static bool isGoodState(EffectInfos Effect)
        {
            switch ((FighterStateEnum)Effect.Value3)
            {
                case FighterStateEnum.STATE_ENRACINER:
                case FighterStateEnum.STATE_INVISIBLE:
                case FighterStateEnum.STATE_REFLECT_SPELL:
                case FighterStateEnum.STATE_SAOUL:
                case FighterStateEnum.STATE_PORTE:
                case FighterStateEnum.STATE_PORTEUR:
                {
                        return true;
                }

                case FighterStateEnum.STATE_AFFAIBLI:
                case FighterStateEnum.STATE_ALTRUISME:
                case FighterStateEnum.STATE_PESANTEUR:
                default:
                {
                        return false;
                }
            }
        }
    }
}
