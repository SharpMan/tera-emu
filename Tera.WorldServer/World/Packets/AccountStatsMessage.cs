using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.Libs.Enumerations;
using Tera.Libs.Network;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.Database.Tables;

namespace Tera.WorldServer.World.Packets
{
    public sealed class AccountStatsMessage : PacketBase
    {
        public Player Character;

        public AccountStatsMessage(Player Character)
        {
            this.Character = Character;
        }

        public override string Compile()
        {
            var Packet = new StringBuilder("As");
            try
            {
                var Stats = Character.myStats;

                Packet.Append(Character.Experience).Append(',')
                      .Append(ExpFloorTable.GetFloorByLevel(Character.Level).Character).Append(',')
                      .Append(ExpFloorTable.GetFloorByLevel(Character.Level + 1).Character).Append('|');
                Packet.Append(Character.Kamas).Append('|');
                Packet.Append(Character.CaractPoint).Append('|');
                Packet.Append(Character.SpellPoint).Append('|');
                Packet.Append(Character.Alignement).Append("~").Append(Character.Alignement).Append(",").Append(Character.AlignementLevel).Append(",")
                      .Append(Character.getGrade()).Append(",").Append(Character.Honor).Append(",").Append(Character.Deshonor + ",")
                      .Append((Character.showWings ? "1" : "0")).Append("|");
                Packet.Append(Character.Life).Append(',').Append(Character.MaxLife).Append('|');
                Packet.Append(Character.Energy).Append(',')
                      .Append(10000).Append('|');
                Packet.Append(Character.Initiative).Append('|');
                Packet.Append(Character.Prospection).Append('|');
                Packet.Append(Stats.GetEffect(EffectEnum.AddPA).ToString()).Append('|');
                Packet.Append(Stats.GetEffect(EffectEnum.AddPM).ToString()).Append('|');
                Packet.Append(Stats.GetEffect(EffectEnum.AddForce).ToString()).Append('|');
                Packet.Append(Stats.GetEffect(EffectEnum.AddVitalite).ToString()).Append('|');
                Packet.Append(Stats.GetEffect(EffectEnum.AddSagesse).ToString()).Append('|');
                Packet.Append(Stats.GetEffect(EffectEnum.AddChance).ToString()).Append('|');
                Packet.Append(Stats.GetEffect(EffectEnum.AddAgilite).ToString()).Append('|');
                Packet.Append(Stats.GetEffect(EffectEnum.AddIntelligence).ToString()).Append('|');
                Packet.Append(Stats.GetEffect(EffectEnum.AddPO).ToString()).Append('|');
                Packet.Append(Stats.GetEffect(EffectEnum.AddInvocationMax).ToString()).Append('|');
                Packet.Append(Stats.GetEffect(EffectEnum.AddDamage).ToString()).Append('|');
                Packet.Append(Stats.GetEffect(EffectEnum.AddDamagePhysic).ToString()).Append('|');
                Packet.Append(Stats.GetEffect(EffectEnum.AddDamageMagic).ToString()).Append('|');
                Packet.Append(Stats.GetEffect(EffectEnum.AddDamagePercent).ToString()).Append('|');
                Packet.Append(Stats.GetEffect(EffectEnum.AddSoins).ToString()).Append('|');
                Packet.Append(Stats.GetEffect(EffectEnum.AddDamagePiege).ToString()).Append('|');
                Packet.Append(Stats.GetEffect(EffectEnum.AddDamagePiege).ToString()).Append('|');
                Packet.Append(Stats.GetEffect(EffectEnum.AddRenvoiDamageItem).ToString()).Append('|');
                Packet.Append(Stats.GetEffect(EffectEnum.AddDamageCritic).ToString()).Append('|');
                Packet.Append(Stats.GetEffect(EffectEnum.AddEchecCritic).ToString()).Append('|');
                Packet.Append(Stats.GetEffect(EffectEnum.AddEsquivePA).ToString()).Append('|');
                Packet.Append(Stats.GetEffect(EffectEnum.AddEsquivePM).ToString()).Append('|');

                /* resistances */
                Packet.Append(Stats.GetEffect(EffectEnum.AddReduceDamageNeutre).ToString()).Append('|');
                Packet.Append(Stats.GetEffect(EffectEnum.AddReduceDamagePourcentNeutre).ToString()).Append('|');
                Packet.Append(Stats.GetEffect(EffectEnum.AddReduceDamagePvPNeutre).ToString()).Append('|');
                Packet.Append(Stats.GetEffect(EffectEnum.AddReduceDamagePourcentPvPNeutre).ToString()).Append('|');

                Packet.Append(Stats.GetEffect(EffectEnum.AddReduceDamageTerre).ToString()).Append('|');
                Packet.Append(Stats.GetEffect(EffectEnum.AddReduceDamagePourcentTerre).ToString()).Append('|');
                Packet.Append(Stats.GetEffect(EffectEnum.AddReduceDamagePvPTerre).ToString()).Append('|');
                Packet.Append(Stats.GetEffect(EffectEnum.AddReduceDamagePourcentPvPTerre).ToString()).Append('|');

                Packet.Append(Stats.GetEffect(EffectEnum.AddReduceDamageEau).ToString()).Append('|');
                Packet.Append(Stats.GetEffect(EffectEnum.AddReduceDamagePourcentEau).ToString()).Append('|');
                Packet.Append(Stats.GetEffect(EffectEnum.AddReduceDamagePvPEau).ToString()).Append('|');
                Packet.Append(Stats.GetEffect(EffectEnum.AddReduceDamagePourcentPvPEau).ToString()).Append('|');

                Packet.Append(Stats.GetEffect(EffectEnum.AddReduceDamageAir).ToString()).Append('|');
                Packet.Append(Stats.GetEffect(EffectEnum.AddReduceDamagePourcentAir).ToString()).Append('|');
                Packet.Append(Stats.GetEffect(EffectEnum.AddReduceDamagePvPAir).ToString()).Append('|');
                Packet.Append(Stats.GetEffect(EffectEnum.AddReduceDamagePourcentPvPAir).ToString()).Append('|');

                Packet.Append(Stats.GetEffect(EffectEnum.AddReduceDamageFeu).ToString()).Append('|');
                Packet.Append(Stats.GetEffect(EffectEnum.AddReduceDamagePourcentFeu).ToString()).Append('|');
                Packet.Append(Stats.GetEffect(EffectEnum.AddReduceDamagePvPFeu).ToString()).Append('|');
                Packet.Append(Stats.GetEffect(EffectEnum.AddReduceDamagePourcentPvPFeu).ToString()).Append('|');

                Packet.Append('1');
            }
            catch (Exception e) { Logger.Error(e); return "BN"; }
            return Packet.ToString();
        }
    }
}
