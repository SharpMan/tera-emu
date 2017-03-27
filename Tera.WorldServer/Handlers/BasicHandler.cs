using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.Libs.Enumerations;
using Tera.WorldServer.Database.Tables;
using Tera.WorldServer.Network;
using Tera.WorldServer.World.Commands;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.Handlers
{
    public static class BasicHandler
    {
        public static void ProcessPacket(WorldClient Client, string Packet)
        {
            switch (Packet[1])
            {
                case 'A':
                    BasicHandler.ProcessBasicConsoleRequest(Client, Packet);
                    break;
                case 'a':
                    BasicHandler.ProcessBasicGeolocationRequest(Client, Packet);
                    break;
                case 'D':
                    BasicHandler.ProcessBasicDateRequest(Client);
                    break;

                case 'S':
                    BasicHandler.ProccesBasicEmoticoneRequest(Client, Packet);
                    break;

                case 'M':
                    BasicHandler.ProcessBasicMessageRequest(Client, Packet);
                    break;
            }
        }

        private static void ProcessBasicGeolocationRequest(WorldClient Client, string Packet)
        {
            if (!Packet.StartsWith("BaM") || Client.Account.Level < 1)
            {
                return;
            }
            String[] pos = Packet.Substring(3).Split(',');
            var map = MapTable.Cache.Values.FirstOrDefault(value => value.X == int.Parse(pos[0]) && value.Y == int.Parse(pos[1]));
            if (map != null)
            {
                if (!map.myInitialized)
                    map.Init();
                Client.Character.Teleport(map, map.getRandomCell());
            }
        }

        private static void ProccesBasicEmoticoneRequest(WorldClient Client, string Packet)
        {
            int id;
            if (int.TryParse(Packet.Substring(2), out id))
            {
                //ifFight
                Client.Character.myMap.SendToMap(new MapEmoticoneMessage(Client.Character.ID, id));
            }
            else
            {
                Client.Send(new BasicNoOperationMessage());
            }
             
        }


        private static void ProcessBasicConsoleRequest(WorldClient Client, String packet)
        {
            if (Client.Account.Level > 0)
            {
                var data = packet.Substring(2).Split(' ');
                var parameters = new CommandParameters(data);
                var command = AdminCommandManager.GetCommand(parameters.Prefix.ToLower());
                if (command != null)
                {
                    command.PreExecute(Client, parameters);
                }
                else
                {
                    Client.Send(new ConsoleMessage("Commande invalide", ConsoleColorEnum.RED));
                }
            }
            else
            {
                Client.Send(new ConsoleMessage("Votre compte n'est pas autoriser a executer ce type de commande", ConsoleColorEnum.RED));
            }
        }

        public static void ProcessBasicDateRequest(WorldClient Client)
        {
            Client.Send(new BasicDateMessage());
        }

        public static void ProcessBasicMessageRequest(WorldClient Client, string Packet)
        {
            var Data = Packet.Substring(2).Split('|');

            var Message = Data[1];

            if (Enum.IsDefined(typeof(ChatChannelEnum), (int)Data[0][0]))
            {
                var Channel = (ChatChannelEnum)Data[0][0];

                if (Client.Character.IsChatChannelEnable(Channel))
                {
                    Client.RaiseChatMessage(Channel, Message);
                }

                Client.Send(new BasicNoOperationMessage());
            }
            else
            {
                var CharacterName = Data[0];

                var Character = CharacterTable.GetCharacter(CharacterName);

                // Le personnage existe ?
                if (Character != null && Character.IsOnline())
                {
                    if (Character.Account != null && Character.Account.Data != null && Character.Account.Data.EnnemyList.ContainsKey(Client.Account.ID))
                    {
                        Client.Send(new TextInformationMessage(TextInformationTypeEnum.ERREUR, 14, Character.Name));
                        return;
                    }
                    Client.Send(new ChatChannelMessage(ChatChannelEnum.CHANNEL_PRIVATE_SEND, Character.ID, Character.Name, Message));
                    Character.Send(new ChatChannelMessage(ChatChannelEnum.CHANNEL_PRIVATE_RECIEVE, Client.Character.ID, Client.Character.Name, Message));
                }
                else
                {
                    // Personnage non trouvé.
                    Client.Send(new ChatMessageErrorMessage(ChatChannelErrorEnum.ERROR_NOT_FOUND));
                }
            }
        }
    }
}
