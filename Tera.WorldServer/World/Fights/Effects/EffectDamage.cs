using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.Libs.Enumerations;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.World.Fights.Effects
{
    /// <summary>
    /// Classe de gestion des dommages terre
    /// </summary>
    public sealed class EffectDamage : EffectBase
    {
        public override int ApplyEffect(EffectCast CastInfos)
        {            
            // Si > 0 alors c'est un buff
            if (CastInfos.Duration > 0)
            {
                // L'effet est un poison
                CastInfos.IsPoison = true;

                // Ajout du buff
                foreach (var Target in CastInfos.Targets)
                {
                    Target.Buffs.AddBuff(new BuffDamage(CastInfos, Target));
                }
            }
            else // Dommage direct
            {
                foreach (var Target in CastInfos.Targets)
                {
                    //Eppe de iop ?
                    if (CastInfos.SpellId == 160 && Target == CastInfos.Caster)
                        continue;
                    var DamageValue = CastInfos.GenerateJet(Target);

                    if (EffectDamage.ApplyDamages(CastInfos, Target, ref DamageValue) == -3)
                        return -3;
                }
            }

            return -1;
        }


        /// <summary>
        /// Applique les dommages.
        /// </summary>
        /// <param name="CastInfos"></param>
        /// <param name="DamageJet"></param>
        public static int ApplyDamages(EffectCast CastInfos, Fighter Target, ref int DamageJet)
        {
            //Esprit Felin
            if (CastInfos.SpellId == 108 && CastInfos.Chance > 0)
            {
                Target = CastInfos.Caster;
            }
            if (Target.States.HasState(FighterStateEnum.STATE_REFLECT_SPELL) && !CastInfos.IsPoison && ((BuffReflectSpell)Target.States.GetBuffByState(FighterStateEnum.STATE_REFLECT_SPELL)).ReflectLevel >= CastInfos.SpellLevel.Level)
            {
                Target.Fight.SendToFight(new FightGameActionMessage(106, Target.ActorId + "", Target.ActorId + ",1"));
                Target = CastInfos.Caster;
            }
            var Caster = CastInfos.Caster;

            // Perd l'invisibilité s'il inflige des dommages direct
            if (!CastInfos.IsPoison && !CastInfos.IsTrap && !CastInfos.IsReflect)
                Caster.States.RemoveState(FighterStateEnum.STATE_INVISIBLE);

            // Application des buffs avant calcul totaux des dommages, et verification qu'ils n'entrainent pas la fin du combat
            if (!CastInfos.IsPoison && !CastInfos.IsReflect)
            {
                if (Caster.Buffs.OnAttackPostJet(CastInfos, ref DamageJet) == -3)
                    return -3; // Fin du combat

                if (Target.Buffs.OnAttackedPostJet(CastInfos, ref DamageJet) == -3)
                    return -3; // Fin du combat
            }

            // Calcul jet
            Caster.CalculDamages(CastInfos.EffectType, ref DamageJet);

            // Calcul resistances
            Target.CalculReduceDamages(CastInfos.EffectType, ref DamageJet);

            // Reduction des dommages grace a l'armure
            if (DamageJet > 0)
            {
                // Si ce n'est pas des dommages direct on ne reduit pas
                if (!CastInfos.IsPoison && !CastInfos.IsReflect)
                {
                    // Calcul de l'armure par rapport a l'effet
                    var Armor = Target.CalculArmor(CastInfos.EffectType);
                    // Si il reduit un minimum
                    if (Armor != 0)
                    {
                        // XX Reduit les dommages de X
                        Target.Fight.SendToFight(new GameActionMessage((int)GameActionTypeEnum.FIGHT_ARMOR, Target.ActorId, Target.ActorId + "," + Armor));
                        
                        // On reduit
                        DamageJet -= Armor;

                        // Si on suprimme totalement les dommages
                        if (DamageJet < 0) 
                            DamageJet = 0;
                    }
                }
            }

            // Application des buffs apres le calcul totaux et l'armure
            if (!CastInfos.IsPoison && !CastInfos.IsReflect)
            {
                if (Caster.Buffs.OnAttackAfterJet(CastInfos, ref DamageJet) == -3) 
                    return -3; // Fin du combat
                if (Target.Buffs.OnAttackedAfterJet(CastInfos, ref DamageJet) == -3)
                    return -3; // Fin du combat
            }

            // S'il subit des dommages
            if (DamageJet > 0)
            {
                // Si c'est pas un poison ou un renvoi on applique le renvoie
                if (!CastInfos.IsPoison && !CastInfos.IsReflect)
                {
                    var ReflectDamage = Target.ReflectDamage;

                    // Si du renvoi
                    if (ReflectDamage > 0)
                    {
                        Target.Fight.SendToFight(new GameActionMessage((int)EffectEnum.AddRenvoiDamage, Target.ActorId, Target.ActorId + "," + ReflectDamage));

                        // Trop de renvois
                        if (ReflectDamage > DamageJet)
                            ReflectDamage = DamageJet;

                        var SubInfos = new EffectCast(EffectEnum.DamageBrut, 0, 0, 0, 0, 0, 0, 0, Target, null);
                        SubInfos.IsReflect = true;

                        // Si le renvoi de dommage entraine la fin de combat on stop
                        if (EffectDamage.ApplyDamages(SubInfos, Caster, ref ReflectDamage) == -3)
                            return -3;

                        // Dommage renvoyé
                        DamageJet -= ReflectDamage;
                    }
                }
            }

            // Peu pas etre en dessous de 0
            if (DamageJet < 0) DamageJet = 0;

            // Dommages superieur a la vie de la cible
            if (DamageJet > Target.Life)
                DamageJet = Target.Life;

            // Deduit la vie
            Target.Life -= DamageJet;

            // Enois du Packet combat subit des dommages
            Target.Fight.SendToFight(new GameActionMessage((int)GameActionTypeEnum.FIGHT_DAMAGE, Caster.ActorId, Target.ActorId + "," + (-DamageJet).ToString()));

            // Tentative de mort et fin de combat
            return Target.TryDie(Caster.ActorId);
        }


    }
}
