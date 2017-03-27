using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.Libs.Enumerations;
using Tera.Libs.Helper;
using Tera.Libs.Utils;
using Tera.WorldServer.Database;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.Database.Tables;
using Tera.WorldServer.Network;
using Tera.WorldServer.World;
using Tera.WorldServer.World.Fights;
using Tera.WorldServer.World.GameActions;
using Tera.WorldServer.World.GameRequests;
using Tera.WorldServer.World.Maps;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.Handlers
{
    public sealed class GuildHandler
    {
        public static Random Random = new Random();

        public static void ProcessPacket(WorldClient Client, string Packet)
        {
            switch (Packet[1])
            {
                case 'B': // BoostStats
                    GuildHandler.ProcessGuildBoostStatsRequest(Client, Packet);
                    break;

                case 'b': // BoostSpell
                    GuildHandler.ProcessGuildBoostSpellRequest(Client, Packet);
                    break;

                case 'V': // Create Leave
                    GuildHandler.ProcessGuildCreationLeaveRequest(Client);
                    break;

                case 'C': // Create
                    GuildHandler.ProcessGuildCreateRequest(Client, Packet);
                    break;

                case 'K': // Leave
                    GuildHandler.ProcessGuildKickRequest(Client, Packet);
                    break;

                case 'F': // Remove Perceptor
                    GuildHandler.ProcesssRemoveTaxCollector(Client, Packet);
                    break;

                case 'P': // Change Grade
                    GuildHandler.ProcessGuildPromoteMembeRequest(Client, Packet);
                    break;

                case 'H': // PutPerceptor
                    GuildHandler.ProcessGuildPutPerceptorRequest(Client, Packet);
                    break;

                case 'I':
                    switch (Packet[2])
                    {
                        case 'B': // Basic Infos
                            GuildHandler.ProcessGuildBoostInformationsRequest(Client);
                            break;

                        case 'G': // General Infos
                            GuildHandler.ProcessGuildGeneralInfosRequest(Client, Packet);
                            break;

                        case 'M': // Members Infos
                            GuildHandler.ProcessGuildMemberInformationsRequest(Client);
                            break;

                        case 'F': // MountPark Infos
                            if (!Client.Character.HasGuild())
                            {
                                Client.Send(new BasicNoOperationMessage());
                                return;
                            }
                            Client.Send(new GuildMountParkInformationMessage(Client.Character.GetGuild()));
                            break;

                        case 'T': // Perceptor Fight Infos
                            if (!Client.Character.HasGuild())
                            {
                                Client.Send(new BasicNoOperationMessage());
                                return;
                            }
                            Client.Send(new GuildFightInformationsMesssage(Client.Character.GetGuild()));
                            TaxCollector.parseAttaque(Client.GetCharacter(), Client.GetCharacter().GetGuild().ID);
                            TaxCollector.parseDefense(Client.GetCharacter(), Client.GetCharacter().GetGuild().ID);
                            break;
                    }
                    break;

                case 'J':
                    switch (Packet[2])
                    {
                        case 'R':
                            GuildHandler.ProcessGuildJoinRequest(Client, Packet);
                            break;

                        case 'K':
                            GuildHandler.ProcessGuildJoinRequestAccept(Client);
                            break;

                        case 'E':
                            GuildHandler.ProcessGuildJoinRequestDeclin(Client);
                            break;
                    }
                    break;

                case 'T':
                    switch (Packet[2])
                    {
                        case 'J': //Rejoindre
                            GuildHandler.ProcessTaxCollectorJoinFigh(Client, Packet.Substring(2));
                            break;
                    }
                    break;

            }
        }

        private static void ProcessTaxCollectorJoinFigh(WorldClient Client , string Packet)
        {
            String PercoID = IntHelper.toString(int.Parse(Packet.Substring(1)), 36);
            long TiD = -1;
            if (!long.TryParse(PercoID, out TiD))
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }
            var TaxCollector = TaxCollectorTable.GetPerco(TiD);
            if (TaxCollector == null)
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            short MapID = -1;
            try
            {
                MapID = (short)TaxCollector.Mapid;
            }
            catch (Exception e)
            {};

            int CellID = -1;
            try
            {
                CellID = TaxCollector.CellId; 
            }
            catch (Exception e)
            {};

            if (TiD == -1 || TaxCollector.CurrentFight == null || MapID == -1 || CellID == -1)
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            if (Client.GetFight() == null && !Client.GetCharacter().isAaway)
            {
                if (Client.GetCharacter().myMap.Id != MapID)
                {
                    Client.GetCharacter().isJoiningTaxFight = true;
                    Client.GetCharacter().OldPosition = new Couple<Map,int>(Client.Character.myMap,Client.Character.CellId);
                    Client.GetCharacter().Teleport(TaxCollector.Map, CellID);
                    try
                    {
                        System.Threading.Thread.Sleep(700);
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e);
                    }
                }

                if (Client.GetCharacter().HasRestriction(RestrictionEnum.RESTRICTION_IS_TOMBESTONE))
                {
                    Client.Send(new BasicNoOperationMessage());
                    return;
                }

                var Team = TaxCollector.CurrentFight.GetTeam2();

                if (TaxCollector.CurrentFight.CanJoin(Team, Client.GetCharacter()))
                {
                    var Fighter = new CharacterFighter(TaxCollector.CurrentFight, Client);

                    var FightAction = new GameFight(Fighter, TaxCollector.CurrentFight);

                    Client.AddGameAction(FightAction);

                    TaxCollector.CurrentFight.JoinFightTeam(Fighter, Team,false,-1,false);
                }
            }


        }

        private static void ProcesssRemoveTaxCollector(WorldClient Client, string Packet)
        {
            if (!Client.GetCharacter().HasGuild() || !Client.GetCharacter().getCharacterGuild().Can(GuildRightEnum.RIGHT_PUT_PERCEPTOR) || Client.GetFight() != null || Client.GetCharacter().isAaway)
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }
            long TaxID;
            if (!long.TryParse(Packet.Substring(2), out TaxID))
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }
            if (Client.GetCharacter().myMap.GetActor(TaxID) != null && Client.GetCharacter().myMap.GetActor(TaxID).ActorType != World.Maps.GameActorTypeEnum.TYPE_TAX_COLLECTOR)
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }
            var TaxC = Client.GetCharacter().myMap.GetActor(TaxID) as TaxCollector;
            if (TaxC == null || TaxC.inFight > 0)
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            Client.GetCharacter().myMap.DestroyActor(TaxC);
            TaxCollectorTable.TryDeleteTax(TaxC);
            StringBuilder toSend = new StringBuilder("gITM").Append(TaxCollector.parsetoGuild(TaxC.GuildID));
            toSend.Append((char)0x00);
            toSend.Append("gT").Append("R").Append(TaxC.N1).Append(",").Append(TaxC.N2).Append("|");
            toSend.Append(TaxC.Mapid).Append("|");
            toSend.Append(TaxC.Map.X).Append("|").Append(TaxC.Map.Y).Append("|").Append(Client.Character.Name);
            Client.Character.GetGuild().Send(new EmptyMessage(toSend.ToString()));

        }

        private static void ProcessGuildPutPerceptorRequest(WorldClient Client, string Packet)
        {
            if (!Client.GetCharacter().HasGuild() || !Client.GetCharacter().getCharacterGuild().Can(GuildRightEnum.RIGHT_PUT_PERCEPTOR) || Client.GetCharacter().GetGuild().CharactersGuildCache.Count < 10)
            {
                Client.Send(new BasicNoOperationMessage());
            }
            else
            {
                short price = (short)(1000 + 10 * Client.GetCharacter().GetGuild().Level);

                if (Client.GetCharacter().Kamas < price)
                {
                    Client.Send(new TextInformationMessage(TextInformationTypeEnum.ERREUR, 82));
                }
                if (TaxCollectorTable.Cache.Where(x => x.Value.Mapid == Client.Character.myMap.Id).Count() > 0)
                {
                    Client.Send(new ImMessage("1168;1"));
                    return;
                }
                if (Client.Character.myMap.FightCell.Length < 5)
                {
                    Client.Send(new TextInformationMessage(TextInformationTypeEnum.ERREUR, 13));
                }
                if (TaxCollectorTable.Cache.Values.Where(x => x.GuildID == Client.Character.GetGuild().ID).Count() >= Client.Character.GetGuild().PerceptorMaxCount)
                {
                    return;
                }

                short random1 = (short)Random.Next(1, 39);
                short random2 = (short)Random.Next(1, 71);

                TaxCollector perco = new TaxCollector()
                {
                    ActorId = TaxCollectorTable.getNextGuid(),
                    Mapid = Client.Character.myMap.Id,
                    CellId = Client.Character.CellId,
                    Orientation = 3,
                    GuildID = Client.Character.GetGuild().ID,
                    N1 = random1,
                    N2 = random2,
                    ItemList = "",
                    Kamas = 0L,
                    XP = 0L,
                };
                TaxCollectorTable.Add(perco);
                Client.Character.myMap.SpawnActor(perco);
                Client.Send(new GameActorShowMessage(GameActorShowEnum.SHOW_SPAWN, perco));
                Client.Send(new GuildFightInformationsMesssage(Client.Character.GetGuild()));
                Client.Character.GetGuild().Send(new GuildTaxCollectorMessage("S" + perco.N1 + "," + perco.N2 + "|" + perco.Mapid + "|" + perco.Map.X + "|" + perco.Map.Y + "|" + Client.Character.Name));

            }
        }

        private static void ProcessGuildBoostInformationsRequest(WorldClient Client)
        {
            if (!Client.GetCharacter().HasGuild())
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }
            Client.Send(new GuildGeneralBoostInformations(Client.Character.GetGuild()));
        }

        private static void ProcessGuildGeneralInfosRequest(WorldClient Client, string Packet)
        {
            if (!Client.GetCharacter().HasGuild())
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }
            Client.Send(new GuildGeneralInfos(Client.GetCharacter().GetGuild(), Client.GetCharacter()));
        }

        private static void ProcessGuildMemberInformationsRequest(WorldClient Client)
        {
            if (!Client.GetCharacter().HasGuild())
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            Client.Send(new GuildAddMemberInformationMessage(Client.GetCharacter().GetGuild().CharactersGuildCache));
        }

        private static void ProcessGuildCreateRequest(WorldClient Client, string Packet)
        {
            if (!Client.IsGameAction(GameActionTypeEnum.GUILD_CREATE))
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            if (!Packet.Contains('|'))
            {
                Client.Send(new GuildCreateMessage(false, "a"));
                return;
            }

            var GuildInfos = Packet.Substring(2).Split('|');

            if (GuildInfos.Length != 5)
            {
                Client.Send(new GuildCreateMessage(false, "a"));
                return;
            }

            var BackGroundId = StringHelper.EncodeBase36(int.Parse(GuildInfos[0]));
            var BackGroundColor = StringHelper.EncodeBase36(int.Parse(GuildInfos[1]));

            var EmblemId = StringHelper.EncodeBase36(int.Parse(GuildInfos[2]));
            var EmblemColor = StringHelper.EncodeBase36(int.Parse(GuildInfos[3]));

            var FullEmblem = BackGroundId + "," + BackGroundColor + "," + EmblemId + "," + EmblemColor;

            var GuildName = GuildInfos[4];

            if (GuildTable.Contains(GuildName) || GuildName.Length > 20)
            {
                Client.Send(new GuildCreateMessage(false, "an"));
                return;
            }

            var Guild = GuildTable.TryCreateGuild(Client, GuildName, FullEmblem);

            if (Guild == null)
            {
                Client.Send(new GuildCreateMessage(false, "an"));
                return;
            }

            Guild.CharactersGuildCache = new List<CharacterGuild>();

            var member = new CharacterGuild()
            {
                ID = Client.Character.ID,
                Name = Client.Character.Name,
                Level = Client.Character.Level,
                Gfx = Client.Character.Look,
                ExperiencePercent = 0,
                Experience = 0,
                Alignement = Client.Character.Alignement,
                lastConnection = Client.Account.LastConnectionDate,
            };

            Guild.AddPlayer(member, GuildGradeEnum.GRADE_BOSS);
            Guild.Register(Client);
            Client.Character.setCharacterGuild(member);
            CharactersGuildTable.Add(member);

            Client.GetCharacter().getCharacterGuild().SendGuildSettingsInfos();

            Client.EndGameAction(GameActionTypeEnum.GUILD_CREATE);

            Client.GetCharacter().RefreshOnMap();
        }

        private static void ProcessGuildJoinRequest(WorldClient Client, string Packet)
        {
            if (!Client.GetCharacter().HasGuild())
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            string TargetName = Packet.Substring(3);


            //var Target = Client.GetCharacter().GetMap().GetCharacter(TargetName);
            var Target = WorldServer.Network.WorldServer.Clients.FirstOrDefault(x => x.Character != null && x.Character.Name.Trim().ToLower() == TargetName.Trim().ToLower()).Character;

            if (Target == null)
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            var TargetClient = Target.GetClient();

            if (TargetClient == null)
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            if (!Client.CanGameAction(GameActionTypeEnum.BASIC_REQUEST))
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            if (!TargetClient.CanGameAction(GameActionTypeEnum.BASIC_REQUEST))
            {
                Client.Send(new GuildJoinMessage('E', GuildJoinReason.REASON_AWAY));
                return;
            }

            if (TargetClient.GetCharacter().HasGuild())
            {
                Client.Send(new GuildJoinMessage('E', GuildJoinReason.REASON_IN_GUILD));
                return;
            }

            // pas les droits necessaires
            if (!Client.GetCharacter().getCharacterGuild().Can(GuildRightEnum.RIGHT_INVIT))
            {
                Client.Send(new GuildJoinMessage('E', GuildJoinReason.REASON_NO_RIGHT));
                return;
            }

            // Ajout de la requete et action
            var Request = new GuildJoinRequest(Client, TargetClient);
            var RequestAction = new GameRequest(Client.GetCharacter(), Request);

            Client.SetBaseRequest(Request);
            TargetClient.SetBaseRequest(Request);

            Client.AddGameAction(RequestAction);
            TargetClient.AddGameAction(RequestAction);

            var RequesterMessage = new GuildJoinMessage('R', Args: TargetName);
            var RequestedMessage = new GuildJoinMessage('r', Args: Client.GetCharacter().ActorId.ToString() + "|" + Client.GetCharacter().Name + "|" + Client.GetCharacter().GetGuild().Name);

            Client.Send(RequesterMessage);
            Target.Send(RequestedMessage);
        }

        private static void ProcessGuildJoinRequestAccept(WorldClient Client)
        {
            if (!Client.IsGameAction(GameActionTypeEnum.BASIC_REQUEST))
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            if (!(Client.GetBaseRequest() is GuildJoinRequest))
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            if (Client == Client.GetBaseRequest().Requester)
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            if (Client.GetBaseRequest().Accept())
            {
                Client.GetBaseRequest().Requester.Send(new GuildJoinMessage('K', GuildJoinReason.REASON_IN_GUILD, Client.GetCharacter().Name));

                var Guild = Client.GetBaseRequest().Requester.GetCharacter().GetGuild();



                var member = new CharacterGuild()
                {
                    ID = Client.Character.ID,
                    Name = Client.Character.Name,
                    Level = Client.Character.Level,
                    Gfx = Client.Character.Look,
                    ExperiencePercent = 0,
                    Experience = 0,
                    Alignement = Client.Character.Alignement,
                    lastConnection = Client.Account.LastConnectionDate,
                };
                Client.Character.setCharacterGuild(member);
                Guild.Register(Client);
                Guild.AddPlayer(Client.GetCharacter().getCharacterGuild(), GuildGradeEnum.GRADE_ESSAI);


                CharactersGuildTable.Add(member);

                Client.GetCharacter().getCharacterGuild().SendGuildSettingsInfos();

                Client.Send(new GuildJoinMessage('K', GuildJoinReason.REASON_JOIN_ACCEPT));

                Client.EndGameAction(GameActionTypeEnum.BASIC_REQUEST);

                Client.GetCharacter().RefreshOnMap();
            }
        }

        private static void ProcessGuildJoinRequestDeclin(WorldClient Client)
        {
            if (!Client.IsGameAction(GameActionTypeEnum.BASIC_REQUEST))
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            if (!(Client.GetBaseRequest() is GuildJoinRequest))
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            if (Client.GetBaseRequest().Declin())
            {
                Client.GetBaseRequest().Requester.Send(new GuildJoinMessage('E', GuildJoinReason.REASON_JOIN_DECLIN));
                Client.GetBaseRequest().Requested.Send(new GuildJoinMessage('E', GuildJoinReason.REASON_JOIN_DECLIN));

                Client.EndGameAction(GameActionTypeEnum.BASIC_REQUEST);
            }
        }

        private static void ProcessGuildCreationLeaveRequest(WorldClient Client)
        {
            if (!Client.IsGameAction(GameActionTypeEnum.GUILD_CREATE))
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            Client.AbortGameAction(GameActionTypeEnum.GUILD_CREATE);
        }

        private static void ProcessGuildKickRequest(WorldClient Client, string Packet)
        {
            var Guild = Client.GetCharacter().getCharacterGuild().GuildCache;

            if (Guild == null)
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            var TargetName = Packet.Substring(2);

            var Member = Guild.GetMember(TargetName);

            if (Member != null)
            {
                if (Member == Client.GetCharacter().getCharacterGuild())
                {
                    Guild.RemovePlayer(Client.GetCharacter().getCharacterGuild());
                    Client.Character.setCharacterGuild(null);
                    if (Client.Character.GetClient() != null)
                    {
                        Guild.UnRegister(Client.Character.GetClient());
                    }

                    Client.Send(new GuildKickMessage('K', Args: TargetName + "|" + TargetName));
                    Client.Character.RefreshOnMap();
                }
                else
                {
                    if (!Client.GetCharacter().getCharacterGuild().Can(GuildRightEnum.RIGHT_BAN))
                    {
                        Client.Send(new GuildKickMessage('E', GuildKickReason.REASON_NO_RIGHT));
                    }
                    else
                    {
                        if (Member.GradeType == GuildGradeEnum.GRADE_BOSS)
                        {
                            Client.Send(new GuildKickMessage('E', GuildKickReason.REASON_UNKNOW));
                        }
                        else
                        {
                            Guild.RemovePlayer(Member);
                            var perso = CharacterTable.GetCharacter(Member.ID);
                            if (perso == null)
                            {
                                return;
                            }
                            perso.setCharacterGuild(null);
                            if (perso.GetClient() != null)
                            {
                                Guild.UnRegister(perso.GetClient());
                            }
                            Client.Send(new GuildKickMessage('K', Args: Client.GetCharacter().Name + "|" + TargetName));

                            perso.Send(new GuildKickMessage('K', Args: Client.GetCharacter().Name));
                            perso.RefreshOnMap();
                        }
                    }
                }
            }

        }

        private static void ProcessGuildPromoteMembeRequest(WorldClient Client, string Packet)
        {
            if (!Client.GetCharacter().HasGuild())
            {
                Client.Send(new BasicNoOperationMessage());
            }
            else
            {
                var PacketData = Packet.Substring(2).Split('|');

                var TargetId = long.Parse(PacketData[0]);
                var NewRank = (GuildGradeEnum)int.Parse(PacketData[1]);
                var NewExperiencePercent = int.Parse(PacketData[2]);
                var NewRights = int.Parse(PacketData[3]);

                if (!Enum.IsDefined(typeof(GuildGradeEnum), NewRank))
                {
                    Client.Send(new BasicNoOperationMessage());
                }
                else
                {
                    if (NewExperiencePercent < 0)
                        NewExperiencePercent = 0;
                    else if (NewExperiencePercent > 90)
                        NewExperiencePercent = 90;

                    var Member = Client.GetCharacter().getCharacterGuild().GuildCache.GetMember(TargetId);

                    if (Member == null)
                    {
                        Client.Send(new BasicNoOperationMessage());
                    }
                    else
                    {
                        // Si c'est le meneur
                        if (Client.GetCharacter().getCharacterGuild().GradeType == GuildGradeEnum.GRADE_BOSS)
                        {
                            // S'il se change lui même
                            if (Member == Client.GetCharacter().getCharacterGuild())
                            {
                                Member.ExperiencePercent = NewExperiencePercent;
                            }
                            else
                            {
                                // S'il veux metre un mec meneur
                                if (NewRank == GuildGradeEnum.GRADE_BOSS)
                                {
                                    Member.SetGrade(GuildGradeEnum.GRADE_BOSS);
                                    Client.GetCharacter().getCharacterGuild().SetGrade(GuildGradeEnum.GRADE_ESSAI);
                                    Client.GetCharacter().getCharacterGuild().OnResetRights();
                                    Client.GetCharacter().getCharacterGuild().SendGuildSettingsInfos();
                                }
                                else
                                {
                                    Member.SetGrade(NewRank);
                                    Member.ExperiencePercent = NewExperiencePercent;
                                    Member.Restriction = NewRights;
                                }
                            }
                        }
                        else
                        {
                            if (Member.GradeType == GuildGradeEnum.GRADE_BOSS)
                            {
                                if (Client.GetCharacter().getCharacterGuild().Can(GuildRightEnum.RIGHT_SET_ALLXP))
                                {
                                    Member.ExperiencePercent = NewExperiencePercent;
                                }
                            }
                            else
                            {
                                if (Client.GetCharacter().getCharacterGuild().Can(GuildRightEnum.RIGHT_SET_GRADE) && NewRank != GuildGradeEnum.GRADE_BOSS)
                                {
                                    Member.SetGrade(NewRank);
                                }

                                if (Client.GetCharacter().getCharacterGuild().Can(GuildRightEnum.RIGHT_SET_RIGHT) && NewRights != 1)
                                {
                                    Member.Restriction = NewRights;
                                }

                                if (Client.GetCharacter().getCharacterGuild().Can(GuildRightEnum.RIGHT_SET_ALLXP) || (Client.GetCharacter().getCharacterGuild().Can(GuildRightEnum.RIGHT_SET_XP) && Member == Client.GetCharacter().getCharacterGuild()))
                                {
                                    Member.ExperiencePercent = NewExperiencePercent;
                                }
                            }
                        }

                        // Renvoi de informations
                        Member.SendGuildSettingsInfos();
                        Client.Send(new GuildAddMemberInformationMessage(Client.GetCharacter().GetGuild().CharactersGuildCache));
                    }
                }
            }
        }

        public static void ProcessGuildBoostStatsRequest(WorldClient Client, string Packet)
        {
            if (Client.GetCharacter().HasGuild())
            {
                var StatsId = Packet[2];

                if (Client.GetCharacter().GetGuild().BoostStats(Client.GetCharacter().getCharacterGuild(), StatsId))
                {
                    Client.Send(new GuildBasicInformationMessage(Client.GetCharacter().GetGuild()));
                }
            }
        }

        public static void ProcessGuildBoostSpellRequest(WorldClient Client, string Packet)
        {
            if (Client.GetCharacter().HasGuild())
            {
                var SpellId = 0;

                if (int.TryParse(Packet.Substring(2), out SpellId))
                {
                    if (Client.GetCharacter().GetGuild().BoostSpell(Client.GetCharacter().getCharacterGuild(), SpellId))
                    {
                        Client.Send(new GuildBasicInformationMessage(Client.GetCharacter().GetGuild()));
                    }
                }
            }
        }

        public static void ProcessGuildBasicInformationRequest(WorldClient Client)
        {
            if (Client.GetCharacter().HasGuild())
            {
                Client.Send(new GuildBasicInformationMessage(Client.GetCharacter().GetGuild()));
            }
        }
    }
}
