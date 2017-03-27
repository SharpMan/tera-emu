using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Enumerations;
using Tera.WorldServer.Network;
using Tera.WorldServer.World;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.Handlers
{
    public static class ConsoleHandler
    {
        public static void ProcessPacket(WorldClient Client, string Packet)
        {
            switch (Packet[1])
            {
                case 'C':
                    ConsoleHandler.ProcessChatChannelEnablingRequest(Client, Packet);
                    break;
            }
        }

        public static void ProcessChatChannelEnablingRequest(WorldClient Client, string Packet)
        {
            var Data = Packet.Substring(2);
            var Register = Data[0] == '+';
            var Channel = (ChatChannelEnum)Data[1];

            if (Register)
            {
                Client.Character.EnableChatChannel(Channel);

                if (Channel == ChatChannelEnum.CHANNEL_GENERAL)
                {
                    if (Client.IsGameAction(GameActionTypeEnum.FIGHT))
                    {
                        Client.GetFight().RegisterToChat(Client);
                    }
                    else
                        Client.Character.myMap.RegisterToChat(Client);
                }
                else if(Channel == ChatChannelEnum.CHANNEL_ALIGNMENT)
                        Network.WorldServer.GetChatController().RegisterClient(Client, Client.Character.AlignmentType);
                else
                    WorldServer.Network.WorldServer.GetChatController().RegisterClient(Client, Channel);
            }
            else
            {
                Client.Character.DisableChatChannel(Channel);
                Client.UnRegisterChatChannel(Channel);
            }

            Client.Send(new BasicNoOperationMessage());
        }
    }
}
