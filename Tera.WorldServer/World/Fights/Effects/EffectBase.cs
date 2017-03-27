using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Enumerations;

namespace Tera.WorldServer.World.Fights.Effects
{
    public abstract class EffectBase
    {
        private static Dictionary<EffectEnum, EffectBase> Effects = new Dictionary<EffectEnum, EffectBase>()
        {
            // Domamges/Vol de vie
            { EffectEnum.DamageTerre   , new EffectDamage()        },
            { EffectEnum.DamageNeutre  , new EffectDamage()        },
            { EffectEnum.DamageFeu     , new EffectDamage()        },
            { EffectEnum.DamageEau     , new EffectDamage()        },
            { EffectEnum.DamageAir     , new EffectDamage()        },
            { EffectEnum.DamageLanceur , new EffectDamageLanceur() },
            { EffectEnum.VolNeutre     , new EffectLifeSteal()     },
            { EffectEnum.VolTerre      , new EffectLifeSteal()     },
            { EffectEnum.VolFeu        , new EffectLifeSteal()     },
            { EffectEnum.VolEau        , new EffectLifeSteal()     },
            { EffectEnum.VolAir        , new EffectLifeSteal()     },
            { EffectEnum.VolVie        , new EffectLifeSteal()     },

            // Soin
            { EffectEnum.Heal        , new EffectHeal()      },

            // Teleporation
            { EffectEnum.Teleport    , new EffectTeleport()  },

            // Armure et bouclié feca
            { EffectEnum.AddArmor    , new EffectArmor()     },
            { EffectEnum.AddArmorBis , new EffectArmor()     },

            // Ajout ou reduction PA/PM/PO/Dommages
            { EffectEnum.AddPA           , new EffectStats()        },
            { EffectEnum.AddPO           , new EffectStats()        },
            { EffectEnum.AddPM           , new EffectStats()        },
            { EffectEnum.SubPA           , new EffectStats()        },
            { EffectEnum.SubPM           , new EffectStats()        },
            { EffectEnum.SubPO           , new EffectStats()        },
            { EffectEnum.SubPAEsquivable    , new EffectSubPAEsquive() },
            { EffectEnum.SubPMEsquivable    , new EffectSubPMEsquive() },
            { EffectEnum.AddEsquivePA    , new EffectStats()        },
            { EffectEnum.AddEsquivePM    , new EffectStats()        },
            { EffectEnum.SubEsquivePA    , new EffectStats()        },
            { EffectEnum.SubEsquivePM    , new EffectStats()        },
            { EffectEnum.AddDamagePhysic , new EffectStats()        },


            //Lose LifePer
            { EffectEnum.DamageLifeNeutre, new EffectLifeDamage()       } ,
            { EffectEnum.DamageLifeEau, new EffectLifeDamage()          } ,
            { EffectEnum.DamageLifeTerre, new EffectLifeDamage()        } ,
            { EffectEnum.DamageLifeAir, new EffectLifeDamage()          } ,
            { EffectEnum.DamageLifeFeu, new EffectLifeDamage()          } ,
            { EffectEnum.DamageDropLife, new EffectDamageDropLife()     },

            
            // Caracteristiques Ajout/Reduction
            { EffectEnum.AddForce              , new EffectStats()     },
            { EffectEnum.AddIntelligence       , new EffectStats()     },
            { EffectEnum.AddInvocationMax      , new EffectStats()     },
            { EffectEnum.AddAgilite            , new EffectStats()     },
            { EffectEnum.AddChance             , new EffectStats()     },
            { EffectEnum.AddSagesse            , new EffectStats()     },
            { EffectEnum.AddVie                , new EffectStats()     },
            { EffectEnum.AddVitalite           , new EffectStats()     },
            { EffectEnum.SubForce              , new EffectStats()     },
            { EffectEnum.SubIntelligence       , new EffectStats()     },
            { EffectEnum.SubAgilite            , new EffectStats()     },
            { EffectEnum.SubChance             , new EffectStats()     },
            { EffectEnum.SubSagesse            , new EffectStats()     },
            { EffectEnum.SubVitalite           , new EffectStats()     },           

            // Soins
            { EffectEnum.AddSoins       , new EffectStats()     },
            { EffectEnum.SubSoins       , new EffectStats()     },

            // Resistances ajout/suppressions
            { EffectEnum.AddReduceDamageAir            , new EffectStats()     },
            { EffectEnum.AddReduceDamageEau            , new EffectStats()     },
            { EffectEnum.AddReduceDamageFeu            , new EffectStats()     },
            { EffectEnum.AddReduceDamageNeutre         , new EffectStats()     },
            { EffectEnum.AddReduceDamageTerre          , new EffectStats()     },
            { EffectEnum.SubReduceDamageAir            , new EffectStats()     },
            { EffectEnum.SubReduceDamageEau            , new EffectStats()     },
            { EffectEnum.SubReduceDamageFeu            , new EffectStats()     },
            { EffectEnum.SubReduceDamageNeutre         , new EffectStats()     },
            { EffectEnum.SubReduceDamageTerre          , new EffectStats()     },
           
            { EffectEnum.AddReduceDamagePhysic         , new EffectStats()     },
            { EffectEnum.AddReduceDamageMagic          , new EffectStats()     },

            { EffectEnum.AddReduceDamagePourcentAir       , new EffectStats()     },
            { EffectEnum.AddReduceDamagePourcentEau       , new EffectStats()     },
            { EffectEnum.AddReduceDamagePourcentFeu       , new EffectStats()     },
            { EffectEnum.AddReduceDamagePourcentNeutre    , new EffectStats()     },
            { EffectEnum.AddReduceDamagePourcentTerre     , new EffectStats()     },
            { EffectEnum.SubReduceDamagePourcentAir       , new EffectStats()     },
            { EffectEnum.SubReduceDamagePourcentEau       , new EffectStats()     },
            { EffectEnum.SubReduceDamagePourcentFeu       , new EffectStats()     },
            { EffectEnum.SubReduceDamagePourcentNeutre    , new EffectStats()     },
            { EffectEnum.SubReduceDamagePourcentTerre     , new EffectStats()     },

            // Ajout ou reduction de dommage
            { EffectEnum.AddDamage             , new EffectStats()     },
            { EffectEnum.AddEchecCritic        , new EffectStats()     },
            { EffectEnum.AddDamageCritic       , new EffectStats()     },
            { EffectEnum.AddDamagePercent      , new EffectStats()     },
            { EffectEnum.SubDamagePercent      , new EffectStats()     },
            { EffectEnum.SubDamage             , new EffectStats()     },
            { EffectEnum.SubDamageCritic       , new EffectStats()     },
            { EffectEnum.AddRenvoiDamage       , new EffectStats()     },

            //Renvoie de sorts
            { EffectEnum.ReflectSpell          , new EffectReflectSpell()    },
            
            // Chatiment sacris
            { EffectEnum.AddChatiment          , new EffectChatiment() },

            //Reperage,Perception
            { EffectEnum.Perception            , new EffectUnHide()    },

            // Effet de push back/fear
            { EffectEnum.PushBack              , new EffectPush()      },
            { EffectEnum.PushFront             , new EffectPush()      },
            { EffectEnum.PushFear              , new EffectPushFear()  },

            // Ajout d'un etat / changement de skin
            { EffectEnum.ChangeSkin            , new EffectSkin()      },
            { EffectEnum.AddState              , new EffectAddState()     },
            { EffectEnum.Invisible             , new EffectAddState()     },

            // Vol de statistique
            { EffectEnum.VolForce              , new EffectStatsSteal()},
            { EffectEnum.VolSagesse            , new EffectStatsSteal()},
            { EffectEnum.VolIntell             , new EffectStatsSteal()},
            { EffectEnum.VolAgi                , new EffectStatsSteal()},
            { EffectEnum.VolChance             , new EffectStatsSteal()},
            { EffectEnum.VolPA                 , new EffectStatsSteal()},
            { EffectEnum.VolPM                 , new EffectStatsSteal()},
            { EffectEnum.VolPO                 , new EffectStatsSteal()},

            // Sacrifice
            { EffectEnum.Sacrifice             , new EffectSacrifice() },
            { EffectEnum.Transpose             , new EffectTranspose() },            

            // Derobade
            { EffectEnum.MissBack              , new EffectDerobade()  },

            //FightObject
            { EffectEnum.UseTrap               , new EffectTrap()      },
            { EffectEnum.UseGlyph              , new EffectGlyphe()    },
            { EffectEnum.UseBlyph              , new EffectBlyphe()    },

            // Augmente de X les domamges de base du sort Y
            { EffectEnum.IncreaseSpellDamage   , new EffectIncreaseSpellJetDamage()},

            // Chance Ecaflip
            { EffectEnum.ChanceEcaflip         , new EffectChanceEcaflip()},

            // 1 PA Utilisé = X Pdv perdu

            { EffectEnum.DamagePerPA           , new EffectDamagePerPA() },

            // Debuff
            { EffectEnum.DeleteAllBonus        , new EffectDebuff()    },

            // Perte etat
            { EffectEnum.LostState             , new EffectLostState() },

            // Panda
            { EffectEnum.Porter                , new EffectPorter()    },
            { EffectEnum.Lancer                , new EffectLancer()    },

            //Invocations
            { EffectEnum.InvocDouble           , new EffectInvocationDouble()       },
            { EffectEnum.Invocation            , new EffectInvocation()             },
            { EffectEnum.InvocationStatic      , new EffectInvocationStatic()       },

            //Suicide
            { EffectEnum.DieFighter            , new EffectDieFighter() },

            //AlterJet
            { EffectEnum.MaximizeEffects        , new EffectAlterJet() },
            { EffectEnum.MinimizeEffects        , new EffectAlterJet() },
        };

        /// <summary>
        /// Application de l'effet
        /// </summary>
        /// <param name="CastInfos"></param>
        /// <returns></returns>
        public static int TryApplyEffect(EffectCast CastInfos)
        {

            if (!EffectBase.Effects.ContainsKey(CastInfos.EffectType))
                return -1;

            return EffectBase.Effects[CastInfos.EffectType].ApplyEffect(CastInfos);
        }

        /// <summary>
        /// Application de l'effet
        /// </summary>
        /// <param name="Fighter"></param>
        public abstract int ApplyEffect(EffectCast CastInfos);
    }
}
