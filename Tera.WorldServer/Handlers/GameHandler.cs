using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.WorldServer.Network;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.Handlers
{
    public static class GameHandler
    {
        public static void ProcessPacket(WorldClient Client, string Packet)
        {
            switch (Packet[0])
            {
                case 'A':
                    switch (Packet[1])
                    {
                        case 'B':
                            CharacterHandler.ProcessBoostStatsRequest(Client, Packet);
                            break;
                    }
                    break;

                case 'B':
                    BasicHandler.ProcessPacket(Client, Packet);
                    break;

                case 'c':
                    ConsoleHandler.ProcessPacket(Client, Packet);
                    break;
                case 'C':
                    ConquestHandler.ProcessPacket(Client, Packet);
                    break;
                case 'D':
                    DialogHandler.ProcessPacket(Client, Packet);
                    break;

                case 'E':
                    ExchangeHandler.ProcessPacket(Client, Packet);
                    break;

                case 'e':
                    EnvironmentHandler.ProcessPacket(Client, Packet);
                    break;

                case 'f':
                    switch (Packet[1])
                    {
                        case 'D':
                            FightHandler.ProcessFightDetailsRequest(Client, Packet);
                            break;

                        case 'L':
                            Client.Send(new FightListMessage(Client.GetCharacter().myMap.GetFights()));
                            break;

                        case 'N':
                        case 'S':
                        case 'P':
                        case 'H':
                            FightHandler.ProcessToggleLockRequest(Client, Packet);
                            break;
                    }
                    break;

                case 'F':
                    FriendHandler.ProcessPacket(Client, Packet);
                    break;

                case 'G':
                    GameActionHandler.ProcessPacket(Client, Packet);
                    break;

                case 'g':
                    GuildHandler.ProcessPacket(Client, Packet);
                    break;

                case 'i':
                    EnnemyHandler.ProcessPacket(Client, Packet);
                    break;

                case 'P':
                    PartyHandler.ProcessPacket(Client, Packet);
                    break;

                case 'R':
                    MountParkHandler.ProcessPacket(Client, Packet);
                    break;

                case 'S':
                    SpellHandler.ProcessPacket(Client, Packet);
                    break;

                case 'O':
                    ObjectHandler.ProcessPacket(Client, Packet);
                    break;
                case 'W':
                    WayPointHandler.ProcessPacket(Client, Packet);
                    break;

            }

            if (Packet.Length > 3 && Packet.Substring(0, 4).Equals("ping"))
            {
                Client.Send(new PingPacket(false)); 
                return;

            }
            if (Packet.Length > 4 && Packet.Substring(0, 5).Equals("qping"))
            {
                Client.Send(new PingPacket(true));
                return;
            }
        }
    }
}
