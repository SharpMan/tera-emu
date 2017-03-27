using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.WorldServer.Network;
using Tera.Libs.Enumerations;
using Tera.WorldServer.Utils;
using Tera.WorldServer.World.Packets;
using Tera.Libs;

namespace Tera.WorldServer.Handlers
{
    public static class GameCreateHandler
    {
        public static void ProcessPacket(WorldClient Client, string Packet)
        {
            switch (Packet)
            {
                case "DB":
                    BasicHandler.ProcessBasicDateRequest(Client);
                    break;

                case "GC1":
                    GameCreateHandler.ProcessGameCreateRequest(Client);
                    break;
            }
            if (Packet.StartsWith("FRGE"))
            {
                Client.hasFightMessage = true;
            }
        }

        public static void ProcessGameCreateRequest(WorldClient Client)
        {
            Client.SetState(WorldState.STATE_GAME_INFORMATION);

            using (CachedBuffer Buffer = new CachedBuffer(Client))
            {
                Buffer.Append(new GameCreateMessage());
                Buffer.Append(new MapDataMessage(Client.Character.myMap));
                Buffer.Append(new AccountStatsMessage(Client.Character));
            }
            if (Client.hasFightMessage)
            {
                if (Client.Character.FightType == -1)
                {
                    return;
                }
                if (Client.Character.FightType == -3)
                {
                    if (Client.Character.OldPosition != null)
                    {
                        Client.Character.Teleport(Client.Character.OldPosition.first, Client.Character.OldPosition.second);
                    }
                    Client.Send(new GameLeaveMessage());
                    Client.SetState(WorldState.STATE_GAME_CREATE);
                    Client.hasFightMessage = false;
                    Client.Character.OldPosition = null;
                    Client.Character.FightType = -1;
                    return;
                }
                if (Client.Character.FightType == -2)
                {
                    Client.Character.WarpToSavePos();
                    Client.Send(new GameLeaveMessage());
                    Client.SetState(WorldState.STATE_GAME_CREATE);
                    Client.hasFightMessage = false;
                    Client.Character.FightType = -1;
                    return;
                }
                Client.Character.myMap.applyEndFightAction((int)Client.Character.FightType, Client.Character);
                Client.Send(new GameLeaveMessage());
                Client.SetState(WorldState.STATE_GAME_CREATE);
                Client.hasFightMessage = false;
                Client.Character.FightType = -1;
            }
        }
    }
}
