using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.WorldServer.Network;
using Tera.Libs.Enumerations;

namespace Tera.WorldServer.Handlers
{
    public sealed class PacketProcessor
    {
        public static void ProcessPacket(WorldClient Client, string Packet)
        {
            try
            {
                switch (Client.GetState())
                {
                    case WorldState.STATE_NON_AUTHENTIFIED:
                        AuthHandler.ProcessAuth(Client, Packet);
                        break;

                    case WorldState.STATE_AUTHENTIFIED:
                        CharacterScreenHandler.ProcessPacket(Client, Packet);
                        break;

                    case WorldState.STATE_CHARACTER_SELECTION:
                        CharacterSelectionHandler.ProcessPacket(Client, Packet);
                        break;

                    case WorldState.STATE_GAME_CREATE:
                        GameCreateHandler.ProcessPacket(Client, Packet);
                        break;

                    case WorldState.STATE_GAME_INFORMATION:
                        GameInformationHandler.ProcessPacket(Client, Packet);
                        break;

                    case WorldState.STATE_IN_GAME:
                        GameHandler.ProcessPacket(Client, Packet);
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("PacketProcessor::ProcessPacket() unknow error "+ ex.ToString());
            }
        }
    }
}
