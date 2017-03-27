using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.Libs.Network;
using Tera.WorldServer.Database.Tables;
using Tera.WorldServer.World.Fights;
using Tera.WorldServer.World.Fights.Fighters;
using Tera.WorldServer.World.Maps;

namespace Tera.WorldServer.World.Packets
{
    public sealed class GameFightEndMessage : PacketBase
    {
        public GameFightEndResult GameEndResult;

        public GameFightEndMessage(GameFightEndResult Result)
        {
            this.GameEndResult = Result;

        }
        public override string Compile()
        {
            var Packet = new StringBuilder("GE");

            Packet.Append((Program.currentTimeMillis() - this.GameEndResult.Fight.startTime)).Append('|'); // FightTime;
            Packet.Append(this.GameEndResult.Fight.FightId).Append('|');
            Packet.Append(this.GameEndResult.Fight.FightType == FightType.TYPE_AGGRESSION || this.GameEndResult.Fight.FightType == FightType.TYPE_PVMA ? 1 : 0); // Can WinHonor ?

            foreach (var Result in this.GameEndResult.FightResults.Values)
            {
                Packet.Append('|').Append(Result.Win ? '2' : '0').Append(';');
                Packet.Append(Result.Fighter.ActorId).Append(';');
                Packet.Append(Result.Fighter.Name).Append(';');
                Packet.Append(Result.Fighter.Level).Append(';');
                Packet.Append(Result.Fighter.Dead ? '1' : '0').Append(';');

                if (this.GameEndResult.Fight.FightType == FightType.TYPE_AGGRESSION || this.GameEndResult.Fight.FightType == FightType.TYPE_PVMA)
                {
                    if (Result.Fighter.ActorType == GameActorTypeEnum.TYPE_CHARACTER)
                    {
                        var CharacterFighter = Result.Fighter as CharacterFighter;

                        Packet.Append(CharacterFighter.Character.AlignmentType != Libs.Enumerations.AlignmentTypeEnum.ALIGNMENT_NEUTRAL ? ExpFloorTable.GetFloorByLevel(CharacterFighter.Character.getGrade()).PvP : 0).Append(';');
                        Packet.Append(CharacterFighter.Character.Honor).Append(';');
                        Packet.Append(CharacterFighter.Character.AlignmentType != Libs.Enumerations.AlignmentTypeEnum.ALIGNMENT_NEUTRAL ? ExpFloorTable.GetFloorByLevel(CharacterFighter.Character.getGrade() + 1).PvP != -1 ? ExpFloorTable.GetFloorByLevel(CharacterFighter.Character.getGrade() + 1).PvP : ExpFloorTable.GetFloorByLevel(CharacterFighter.Character.getGrade()).PvP : 0).Append(';');
                        Packet.Append(Result.WinHonor).Append(';');
                        Packet.Append(CharacterFighter.Character.getGrade()).Append(';');
                        Packet.Append(CharacterFighter.Character.Deshonor).Append(';');
                        Packet.Append(Result.WinDisHonor).Append(';');
                        if (Result.WinItems != null)
                            Packet.Append(string.Join(",", Result.WinItems.Select(x => x.Key + "~" + x.Value))).Append(';');
                        else
                            Packet.Append("").Append(';');

                        Packet.Append(Result.WinKamas.ToString()).Append(';');
                        Packet.Append(ExpFloorTable.GetFloorByLevel(Result.Fighter.Level).Character).Append(';');    
                        Packet.Append((Result.Fighter as CharacterFighter).Character.Experience).Append(';');
                        Packet.Append(ExpFloorTable.GetFloorByLevel(Result.Fighter.Level + 1).Character).Append(';');
                        Packet.Append(Result.WinExp);
                    }
                    else if (Result.Fighter.ActorType == GameActorTypeEnum.TYPE_PRISM)
                    {
                        var CharacterFighter = Result.Fighter as PrismFighter;

                        Packet.Append(CharacterFighter.Prisme.AlignmentType != Libs.Enumerations.AlignmentTypeEnum.ALIGNMENT_NEUTRAL ? ExpFloorTable.GetFloorByLevel(CharacterFighter.Prisme.Level).PvP : 0).Append(';');
                        Packet.Append(CharacterFighter.Prisme.Honor).Append(';');
                        Packet.Append(CharacterFighter.Prisme.AlignmentType != Libs.Enumerations.AlignmentTypeEnum.ALIGNMENT_NEUTRAL ? ExpFloorTable.GetFloorByLevel(CharacterFighter.Prisme.Level + 1).PvP != -1 ? ExpFloorTable.GetFloorByLevel(CharacterFighter.Prisme.Level + 1).PvP : ExpFloorTable.GetFloorByLevel(CharacterFighter.Prisme.Level).PvP : 0).Append(';');
                        Packet.Append(Result.WinHonor).Append(';');
                        Packet.Append(CharacterFighter.Prisme.Level).Append(';');
                        Packet.Append(0).Append(';');
                        Packet.Append(Result.WinDisHonor).Append(';');
                        if (Result.WinItems != null)
                            Packet.Append(string.Join(",", Result.WinItems.Select(x => x.Key + "~" + x.Value))).Append(';');
                        else
                            Packet.Append("").Append(';');

                        Packet.Append(Result.WinKamas.ToString()).Append(';');
                        Packet.Append(0).Append(';');
                        Packet.Append(0).Append(';');
                        Packet.Append(0).Append(';');
                        Packet.Append(Result.WinExp);
                    }
                }
                else
                {
                    if (Result.Fighter.ActorType == GameActorTypeEnum.TYPE_CHARACTER)
                    {
                        Packet.Append(ExpFloorTable.GetFloorByLevel(Result.Fighter.Level).Character).Append(';');
                        Packet.Append(this.GameEndResult.Fight.FightType == FightType.TYPE_KOLIZEUM ? (Result.Fighter as CharacterFighter).Character.Cote : (Result.Fighter as CharacterFighter).Character.Experience).Append(';');
                        Packet.Append(this.GameEndResult.Fight.FightType == FightType.TYPE_KOLIZEUM ? 10000 : ExpFloorTable.GetFloorByLevel(Result.Fighter.Level + 1).Character).Append(';');
                        Packet.Append(Result.WinExp).Append(';');
                        Packet.Append(Result.WinGuildXp).Append(';');
                        Packet.Append(Result.WinMountXp).Append(';');
                    }
                    else
                    {
                        Packet.Append(";;;;;;");
                    }

                    if (Result.WinItems != null)
                        Packet.Append(string.Join(",", Result.WinItems.Select(x => x.Key + "~" + x.Value))).Append(';');
                    else
                        Packet.Append("").Append(';');

                    Packet.Append(Result.WinKamas > 0 ? Result.WinKamas.ToString() : "");
                }
            }

            

            if (GameEndResult.TCollectorResult != null)
            {
                Packet.Append("|5;").Append(GameEndResult.TCollectorResult.TaxCollector.ActorId).Append(";").Append(GameEndResult.TCollectorResult.TaxCollector.N1).Append(",").Append(GameEndResult.TCollectorResult.TaxCollector.N2).Append(";").Append(GameEndResult.TCollectorResult.TaxCollector.Guild.Level).Append(";0;");
                Packet.Append(GameEndResult.TCollectorResult.TaxCollector.Guild.Level).Append(";");
                Packet.Append(GameEndResult.TCollectorResult.TaxCollector.Guild.Experience).Append(";");
                Packet.Append(ExpFloorTable.getGuildXpMax(GameEndResult.TCollectorResult.TaxCollector.Guild.Level)).Append(";;");
                Packet.Append(GameEndResult.TCollectorResult.WinExp).Append(";;");
                if (GameEndResult.TCollectorResult.WinItems != null)
                    Packet.Append(string.Join(",", GameEndResult.TCollectorResult.WinItems.Select(x => x.Key + "~" + x.Value))).Append(';');
                else
                    Packet.Append("").Append(';');
                Packet.Append(GameEndResult.TCollectorResult.WinKamas).Append("|");
            }
            
            return Packet.ToString();
        }
    }
}
