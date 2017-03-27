using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Enumerations;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.World.Fights.Effects
{
    /// <summary>
    /// Classe de gestion des soins
    /// </summary>
    public sealed class EffectHeal : EffectBase
    {
        /// <summary>
        /// Application de l'effet, deux sorte : buff, direct
        /// </summary>
        /// <param name="CastInfos"></param>
        /// <returns></returns>
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
                    Target.Buffs.AddBuff(new BuffHeal(CastInfos, Target));
                }
            }
            else // Heal direct
            {
                foreach (var Target in CastInfos.Targets)
                    if (EffectHeal.ApplyHeal(CastInfos, Target, CastInfos.GenerateJet(Target)) == -3)
                        return -3;
            }

            return -1;
        }

        /// <summary>
        /// Application du heal
        /// </summary>
        /// <param name="CastInfos"></param>
        /// <param name="Heal"></param>
        /// <returns></returns>
        public static int ApplyHeal(EffectCast CastInfos, Fighter Target, int Heal)
        {
            var Caster = CastInfos.Caster;

            // Boost soin etc
            Caster.CalculHeal(ref Heal);

            // Si le soin est superieur a sa vie actuelle
            if (Target.Life + Heal > Target.MaxLife)
                Heal = Target.MaxLife - Target.Life;

            // Affectation
            Target.Life += Heal;

            // Envoi du Packet
            Target.Fight.SendToFight(new GameActionMessage((int)GameActionTypeEnum.FIGHT_HEAL, Caster.ActorId, Target.ActorId + "," + Heal));
            
            // Le soin entraine la fin du combat ?
            return Target.TryDie(Caster.ActorId);
        }
    }
}
