using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;
using Tera.WorldServer.World.Fights;

namespace Tera.WorldServer.World.Packets
{
    public sealed class GameFightFlagDisplayMessage : PacketBase
    {
        public Fight Fight;

        public GameFightFlagDisplayMessage(Fight Fight)
        {
            this.Fight = Fight;
        }

        public override string Compile()
        {
            var Packet = new StringBuilder("Gc+");

            this.Fight.SerializeAs_FlagDisplayInformations(Packet);

            return Packet.ToString();
        }
    }
}
