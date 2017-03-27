using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Enumerations;
using Tera.WorldServer.Network;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.Handlers
{
    public static class CharacterScreenHandler
    {
        public static void ProcessPacket(WorldClient Client, string Packet)
        {
            switch (Packet)
            {
                case "AV":
                    Client.Send(new UnknowAV0Message());
                    break;

                case "AL":
                    ProcessCharacterListRequest(Client);
                    break;
            }
        }

        private static void ProcessCharacterListRequest(WorldClient Client)
        {
            // En attente de selection d'un personnage
            Client.SetState(WorldState.STATE_CHARACTER_SELECTION);

            // envoie de la liste des personnages
            Client.Send(new CharactersListMessage(Client.Account.Characters));
        }
    }
}
