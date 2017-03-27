using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Enumerations;
using Tera.WorldServer.Network;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.Handlers
{
    public static class AlignmentHandler
    {
        public static void ProcessAlignmentEnableRequest(WorldClient Client, string Packet)
        {
            if (Client.GetCharacter().AlignmentType == AlignmentTypeEnum.ALIGNMENT_NEUTRAL || Packet.Length < 2)
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            int Hloose = Client.GetCharacter().Honor * 5 / 100;

            switch (Packet[2])
            {
                case '*':
                    Client.Send(new CharacterAlignementLose(Hloose));
                    return;
                case '+':
                    Client.GetCharacter().showWings = true;
                    break;
                case '-':
                    Client.GetCharacter().showWings = false;
                    Client.GetCharacter().RemoveHonor(Hloose);
                    break;
            }
            Client.GetCharacter().RefreshOnMap();
            Client.Send(new AccountStatsMessage(Client.GetCharacter()));
        }
    }
}
