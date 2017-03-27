using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.WorldServer.Network;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.Handlers
{
    public static class EnvironmentHandler
    {
        public static void ProcessPacket(WorldClient Client, string Packet)
        {
            switch (Packet[1])
            {
                case 'e':
                    EnvironmentHandler.doEmoticoneRequest(Client, Packet);
                    break;
                case 'D':
                    EnvironmentHandler.changeDirectionRequest(Client, Packet);
                    break;
            }
        }

        private static void changeDirectionRequest(WorldClient Client, string Packet)
        {
            try
            {
                if (Client.GetFight() != null)
                {
                    return;
                }
                int dir = int.Parse(Packet.Substring(2));
                Client.Character.Orientation = dir;
                Client.Character.myMap.SendToMap(new MapDirectionChanged(Client.Character.ActorId, dir));
            }
            catch (Exception e)
            {
                return;
            };
        }

        private static void doEmoticoneRequest(WorldClient Client, string Packet)
        {
            int emote = -1;
            try
            {
                emote = int.Parse(Packet.Substring(2));
            }
            catch (Exception e)
            {
            };
            if (emote == -1)
            {
                return;
            }
            if (Client.GetFight() != null) return;
            switch (emote)//effets spéciaux des émotes
            {
                case 19://s'allonger 
                case 1:// s'asseoir
                    //_perso.setSitted(!_perso.isSitted());
                    break;
            }
            if (Client.Character.emoteActive() == emote)
            {
                Client.Character.setEmoteActive(0);
            }
            else
            {
                Client.Character.setEmoteActive(emote);
            }

            Client.Character.myMap.SendToMap(new MapEnvironementEmoticoneMessage(Client.Character.ActorId,Client.Character.emoteActive()));

        }
    }
}
