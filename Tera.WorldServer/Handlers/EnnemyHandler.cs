using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.Database.Tables;
using Tera.WorldServer.Network;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.Handlers
{
    public static class EnnemyHandler
    {
        public static void ProcessPacket(WorldClient Client, string Packet)
        {
            switch (Packet[1])
            {
                case 'A':
                    EnnemyHandler.ProcessEnnemyAddRequest(Client, Packet);
                    break;
                case 'L':
                    Client.Send(new CharacterEnnemyListMessage(Client.GetCharacter()));
                    break;
                case 'D':
                    EnnemyHandler.ProcessEnnemyDeleteRequest(Client, Packet);
                    break;
                default:
                    Client.Send(new BasicNoOperationMessage());
                    break;
            }
        }

        private static void ProcessEnnemyDeleteRequest(WorldClient Client, string Packet)
        {
            int guid = -1;
            switch (Packet[2])
            {
                case '%':
                    Packet = Packet.Substring(3);
                    Player p = CharacterTable.GetCharacter(Packet);
                    if (p == null || !p.IsOnline())
                    {
                        Client.Send(new CharacterEnnemyDeleteMessage("Ef"));
                        return;
                    }
                    guid = p.GetClient().Account.ID;
                    break;
                case '*': 
                    Packet = Packet.Substring(3);
                    var cible = Client.Account.Data.EnnemyList.Where(x => x.Value.Equals(Packet));

                    if (cible != null && cible.Count() > 0)
                        guid = cible.First().Key;
                    else
                    {
                        Client.Send(new CharacterEnnemyDeleteMessage("Ef"));
                        return;
                    }
                    var ClientCibled = Network.WorldServer.Clients.First(x => x.Account != null && x.Account.ID == guid);
                    if (ClientCibled.Account == null || ClientCibled.Character == null || !ClientCibled.Character.IsOnline())
                    {
                        Client.Send(new CharacterEnnemyDeleteMessage("Ef"));
                        return;
                    }

                    guid = ClientCibled.Account.ID;
                    break;
                default:
                    Packet = Packet.Substring(2);
                    Player Pr = CharacterTable.GetCharacter(Packet);
                    if (Pr == null || !Pr.IsOnline())
                    {
                        Client.Send(new CharacterEnnemyDeleteMessage("Ef"));
                        return;
                    }
                    guid = Pr.GetClient().Account.ID;
                    break;
            }
            if (guid == -1 || !Client.Account.Data.EnnemyList.ContainsKey(guid))
            {
                Client.Send(new CharacterEnnemyDeleteMessage("Ef"));
                return;
            }
            Client.Account.Data.EnnemyList.Remove(guid);
            Client.Account.Data.Save();
            Client.Send(new EnnemyDeleteOkMessage("K"));
        }

        private static void ProcessEnnemyAddRequest(WorldClient Client, string Packet)
        {
            WorldClient client = null;
            switch (Packet[2])
            {
                case '%':
                    Packet = Packet.Substring(3);
                    Player P = CharacterTable.GetCharacter(Packet);
                    if (P == null || !P.IsOnline())
                    {
                        Client.Send(new CharacterFriendAddMessage("Ef"));
                        return;
                    }
                    client = P.GetClient();
                    break;
                case '*': // Pseudo
                    Packet = Packet.Substring(3);
                    var sock = WorldServer.Network.WorldServer.Clients.FirstOrDefault(x => x.Account != null && x.Account.Pseudo == Packet);
                    P = null;
                    if (sock != null)
                        P = sock.Character;
                    if (P == null || !P.IsOnline())
                    {
                        Client.Send(new CharacterFriendAddMessage("Ef"));
                        return;
                    }
                    client = P.GetClient();
                    break;
                default:
                    Packet = Packet.Substring(2);
                    Player Pr = CharacterTable.GetCharacter(Packet);
                    if (Pr == null || !Pr.IsOnline())
                    {
                        Client.Send(new CharacterFriendAddMessage("Ef"));
                        return;
                    }
                    client = Pr.GetClient();
                    break;
            }
            if (client == null)
            {
                Client.Send(new CharacterFriendAddMessage("Ef"));
                return;
            }
            if (client.Account.ID == Client.Account.ID)
            {
                Client.Send(new CharacterFriendAddMessage("Ey"));
                return;
            }
            if (!Client.Account.Data.EnnemyList.ContainsKey(client.Account.ID))
            {
                Client.Account.Data.EnnemyList.Add(client.Account.ID, client.Account.Pseudo);
                StringBuilder sb = new StringBuilder(";").Append("?;")/* Chno had zab ?*/.Append(client.Character.Name).Append(";");
                if (client.Account.Data.EnnemyList.ContainsKey(Client.Account.ID))
                {
                    sb.Append(client.Character.Level).Append(";");
                    sb.Append(client.Character.Alignement).Append(";");
                }
                else
                {
                    sb.Append("?;");
                    sb.Append("-1;");
                }
                sb.Append(client.Character.Classe).Append(";");
                sb.Append(client.Character.Sexe).Append(";");
                sb.Append(client.Character.Look);

                Client.Send(new CharacterEnnemyAddMessage(client.Account.Pseudo + sb.ToString()));
                Client.Account.Data.Save();
            }
            else
            {
                Client.Send(new CharacterOldEnnemyAddMessage());
            }
        }
    }
}
