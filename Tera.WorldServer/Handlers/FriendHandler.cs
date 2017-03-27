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
    public static class FriendHandler
    {
        public static void ProcessPacket(WorldClient Client, string Packet)
        {
            switch (Packet[1])
            {
                case 'A':
                    FriendHandler.ProcessFriendAddRequest(Client, Packet);
                    break;
                case 'L':
                    Client.Send(new CharacterFriendListMessage(Client.GetCharacter()));
                    break;
                case 'D':
                    FriendHandler.ProcessFriendDeleteRequest(Client, Packet);
                    break;
                case 'O':
                    switch (Packet[2])
                    {
                        case '-':
                            Client.Account.Data.showFriendConnection = false;
                            Client.Send(new BasicNoOperationMessage());
                            break;
                        case '+':
                            Client.Account.Data.showFriendConnection = true;
                            Client.Send(new BasicNoOperationMessage());
                            break;
                    }
                    break;
            }
        }

        private static void ProcessFriendDeleteRequest(WorldClient Client, string Packet)
        {
            int guid = -1;
            switch (Packet[2])
            {
                case '%':
                    Packet = Packet.Substring(3);
                    Player p = CharacterTable.GetCharacter(Packet);
                    if (p == null || !p.IsOnline())
                    {
                        Client.Send(new CharacterFriendAddMessage("Ef"));
                        return;
                    }
                    guid = p.GetClient().Account.ID;
                    break;
                default:
                    Packet = Packet.Substring(2);
                    Player Pr = CharacterTable.GetCharacter(Packet);
                    if (Pr == null || !Pr.IsOnline())
                    {
                        Client.Send(new CharacterFriendAddMessage("Ef"));
                        return;
                    }
                    guid = Pr.GetClient().Account.ID;
                    break;
            }
            if (guid == -1 || !Client.Account.Data.FriendsList.ContainsKey(guid))
            {
                Client.Send(new CharacterFriendAddMessage("Ef"));
                return;
            }
            Client.Account.Data.FriendsList.Remove(guid);
            Client.Account.Data.Save();
            Client.Send(new CharacterFriendDeleteMessage("K"));
        }

        private static void ProcessFriendAddRequest(WorldClient Client, string Packet)
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
            if (!Client.Account.Data.FriendsList.ContainsKey(client.Account.ID))
            {
                Client.Account.Data.FriendsList.Add(client.Account.ID, client.Account.Pseudo);
                StringBuilder sb = new StringBuilder(";").Append("?;")/* Chno had zab ?*/.Append(client.Character.Name).Append(";");
                if (client.Account.Data.FriendsList.ContainsKey(Client.Account.ID))
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

                Client.Send(new CharacterFriendAddMessage("K" + client.Account.Pseudo + sb.ToString()));
                Client.Account.Data.Save();
            }
            else
            {
                Client.Send(new CharacterFriendAddMessage("Ea"));
            }
        }
    }
}
