using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Enumerations;
using Tera.WorldServer.Network;
using Tera.WorldServer.Utils;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.Handlers
{
    public static class GameInformationHandler
    {
        public static void ProcessPacket(WorldClient Client, string Packet)
        {
            switch (Packet)
            {
                case "BD":
                    BasicHandler.ProcessBasicDateRequest(Client);
                    break;

                case "GI":
                    GameInformationHandler.ProcessGameInformationRequest(Client);
                    break;
            }
        }

        public static void ProcessBasicDateRequest(WorldClient Client)
        {
            Client.Send(new BasicDateMessage());
        }

        public static void ProcessGameInformationRequest(WorldClient Client)
        {
            // On spawn le personnage
            try
            {
                Client.Character.SpawnToMap();
            }
            catch (ArgumentException e)
            {
            }

            // On change le status
            Client.SetState(WorldState.STATE_IN_GAME);

            using (CachedBuffer Buffer = new CachedBuffer(Client))
            {
                if (!Client.GetCharacter().isJoiningTaxFight)
                {
                    if (Client.Character.myMap.mountPark != null)
                    {
                        Buffer.Append(new MapMountParkMessage(Client.Character.myMap.mountPark));
                    }
                    // On envoie la position des joueurs
                    Buffer.Append(new GameMapComplementaryInformationsMessage(Client.Character.myMap.GetActors()));
                }
                else
                {
                    Client.GetCharacter().isJoiningTaxFight = false;
                }
                Buffer.Append(new GameDataOkMessage());
                
            }
        }
    }
}
