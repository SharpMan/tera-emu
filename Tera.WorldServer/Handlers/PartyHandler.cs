using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.Database.Tables;
using Tera.WorldServer.Network;
using Tera.WorldServer.World;
using Tera.WorldServer.World.Character;
using Tera.WorldServer.World.GameActions;
using Tera.WorldServer.World.GameRequests;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.Handlers
{
    public static class PartyHandler
    {
        public static void ProcessPacket(WorldClient Client, string Packet)
        {
            switch (Packet[1])
            {
                case 'I':
                    PartyHandler.ProcessPartyRequest(Client, Packet);
                    break;
                case 'A':
                    PartyHandler.ProcessPartyAccept(Client, Packet);
                    break;
                case 'V':
                    PartyHandler.ProcessPartyLeave(Client, Packet);
                    break;
                case 'R':
                    PartyHandler.ProcessPartyRefuse(Client, Packet);
                    break;
                case 'F':
                    PartyHandler.ProcessFollowMember(Client, Packet);
                    break;
                case 'G':
                    PartyHandler.ProcessFollowAll(Client, Packet);
                    break;
            }
        }

        private static void ProcessFollowAll(WorldClient Client, string Packet)
        {
            if (Client.GetGameAction(GameActionTypeEnum.GROUP) != null)
            {
                var Party = (Client.GetGameAction(GameActionTypeEnum.GROUP) as GameParty).Party;
                long pGuid2 = -1;
                if (!long.TryParse(Packet.Substring(3), out pGuid2))
                {
                    Client.Send(new BasicNoOperationMessage());
                    return;
                }
                var P2 = CharacterTable.GetCharacter(pGuid2);
                if (P2 == null || !P2.IsOnline())
                {
                    Client.Send(new BasicNoOperationMessage());
                    return;
                }
                if (Packet[2] == '+')
                {
                    foreach (var T in Party.Players)
                    {
                        if (T.ActorId == Client.Character.ActorId)
                            continue;
                        if (T.Follows != null)
                        {
                            T.Follows.Follower.Remove(Client.Character.ActorId);
                        }
                        T.Send(new CharacterFlagMessage(P2));
                        T.Send(new PartyFlagMessage("+" + P2.ActorId));
                        T.Follows = P2;
                        P2.Follower.Add(T.ActorId, T);
                    }
                }
                else if (Packet[2] == '-')
                {
                    foreach (var T in Party.Players)
                    {
                        if (T.ActorId == Client.Character.ActorId)
                            continue;
                        T.Send(new CharacterDeleteFlagMessage());
                        T.Send(new PartyFlagMessage("-"));
                        T.Follows = null;
                        P2.Follower.Remove(T.ActorId);
                    }
                }
            }
        }

        private static void ProcessFollowMember(WorldClient Client, string Packet)
        {
            if (Client.GetGameAction(GameActionTypeEnum.GROUP) != null)
            {
                var Party = (Client.GetGameAction(GameActionTypeEnum.GROUP) as GameParty).Party;
                long pGuid = -1;
                if (!long.TryParse(Packet.Substring(3), out pGuid))
                {
                    Client.Send(new BasicNoOperationMessage());
                    return;
                }
                var P = CharacterTable.GetCharacter(pGuid);
                if (P == null || !P.IsOnline())
                {
                    Client.Send(new BasicNoOperationMessage());
                    return;
                }
                if (Packet[2] == '+')
                {
                    if (Client.Character.Follows != null)
                    {
                        Client.Character.Follows.Follower.Remove(Client.Character.ActorId);
                    }
                    Client.Send(new CharacterFlagMessage(P));
                    Client.Send(new PartyFlagMessage("+" + P.ActorId));
                    Client.Character.Follows = P;
                    P.Follower.Add(Client.Character.ActorId, Client.Character);
                }
                else if(Packet[2] == '-')
                {
                    Client.Send(new CharacterDeleteFlagMessage());
                    Client.Send(new PartyFlagMessage("-"));
                    Client.Character.Follows = null;
                    Client.Character.Follower.Remove(Client.Character.ActorId);
                }

            }
        }

        private static void ProcessPartyRefuse(WorldClient Client, string Packet)
        {
            if (!Client.IsGameAction(GameActionTypeEnum.BASIC_REQUEST))
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            if (!(Client.GetBaseRequest() is PartyRequest))
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            if (Client == Client.GetBaseRequest().Requester)
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            if (Client.GetBaseRequest().Declin())
            {
                Client.Send(new BasicNoOperationMessage());
                Client.EndGameAction(GameActionTypeEnum.BASIC_REQUEST);
            }
                
        }

        private static void ProcessPartyLeave(WorldClient Client, string Packet)
        {
            if (!Client.IsGameAction(GameActionTypeEnum.GROUP))
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }
            var Party = (Client.GetGameAction(GameActionTypeEnum.GROUP) as GameParty).Party;
            if (Party == null)
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }
            if (Packet.Length == 2)
            {
                Party.Leave(Client.Character);
                Client.Send(new PartyLeaveMessage(""));
                Client.Send(new InconnuHelpMessage(""));
            }
            else
            {
                long guid = -1;
                if (!long.TryParse(Packet.Substring(2), out guid))
                {
                    Client.Send(new BasicNoOperationMessage());
                    return;
                }
                if (guid == -1)
                {
                    return;
                }
                var Target = CharacterTable.GetCharacter(guid);
                if (Target == null)
                {
                    Client.Send(new BasicNoOperationMessage());
                    return;
                }
                Party.Leave(Target);
                Target.Send(new PartyLeaveMessage(Client.Character.ActorId.ToString()));
                Target.Send(new InconnuHelpMessage(""));
            }
        }

        private static void ProcessPartyAccept(WorldClient Client, string Packet)
        {
            if (!Client.IsGameAction(GameActionTypeEnum.BASIC_REQUEST))
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            if (!(Client.GetBaseRequest() is PartyRequest))
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            if (Client == Client.GetBaseRequest().Requester)
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            // Lancement du combat
            if (Client.GetBaseRequest().Accept())
            {
                if (Client.GetBaseRequest().Requester.GetGameAction(GameActionTypeEnum.GROUP) == null)
                {
                    var Party = new Party(Client.GetBaseRequest().Requester.Character, Client.Character);

                    Client.EndGameAction(GameActionTypeEnum.BASIC_REQUEST);
                    Client.GetBaseRequest().Requester.EndGameAction(GameActionTypeEnum.BASIC_REQUEST);

                    Client.AddGameAction(new GameParty(Client.Character, Party));
                    Client.GetBaseRequest().Requester.AddGameAction(new GameParty(Client.GetBaseRequest().Requester.Character, Party));

                    Client.Send(new PartyCreatedMesssage(Party));
                    Client.Send(new PartyListMessage(Party));

                    Client.GetBaseRequest().Requester.Send(new PartyCreatedMesssage(Party));
                    Client.GetBaseRequest().Requester.Send(new PartyListMessage(Party));

                    Party.Send(new PartyAllGroupMember(Party));
                   
                }
                else
                {
                    var Party = (Client.GetBaseRequest().Requester.GetGameAction(GameActionTypeEnum.GROUP) as GameParty).Party;
                    Client.Send(new PartyCreatedMesssage(Party));
                    Client.Send(new PartyListMessage(Party));

                    Party.Send(new PartyPlayerAddedMessage(Client.Character));

                    Client.EndGameAction(GameActionTypeEnum.BASIC_REQUEST);
                    Client.GetBaseRequest().Requester.EndGameAction(GameActionTypeEnum.BASIC_REQUEST);

                    Client.AddGameAction(new GameParty(Client.Character, Party));
                    Client.Send(new PartyAllGroupMember(Party));
                }
                Client.GetBaseRequest().Requester.Send(new PartyRefuseMessage());
                Client.GetBaseRequest().Requester.SetBaseRequest(null);
                Client.SetBaseRequest(null);
            }
        }

        private static void ProcessPartyRequest(WorldClient Client, string Packet)
        {
            Player Target = CharacterTable.GetCharacter(Packet.Substring(2));
            if (Target == null || !Target.IsOnline()  || Target.isAaway)
            {
                Client.Send(new PartyErrorMessage("n"));
                return;
            }
            if (!Target.Client.CanGameAction(World.GameActionTypeEnum.GROUP))
            {
                Client.Send(new PartyErrorMessage("a"));
                return;
            }
            if (!Client.CanGameAction(GameActionTypeEnum.BASIC_REQUEST))
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }
            if (!Target.Client.CanGameAction(GameActionTypeEnum.BASIC_REQUEST)) 
            {
                Client.Send(new PartyErrorMessage("a"));
                return;
            }
            if (Client.GetBaseRequest() != null && Client == Client.GetBaseRequest().Requester)
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }
            if (Client.IsGameAction(World.GameActionTypeEnum.GROUP) && (Client.GetGameAction(World.GameActionTypeEnum.GROUP) as GameParty).Party.Players.Count == 8)
            {
                Client.Send(new PartyErrorMessage("f"));
                return;
            }

            var Request = new PartyRequest(Client, Target.Client);
            var RequestAction = new GameRequest(Client.GetCharacter(), Request);

            Client.SetBaseRequest(Request);
            Target.Client.SetBaseRequest(Request);

            Client.AddGameAction(RequestAction);
            Target.Client.AddGameAction(RequestAction);

            var Message = new PartyRequestMessage(Client.Character.Name, Target.Name);

            Client.Send(Message);
            Target.Send(Message);

        }
    }
}
