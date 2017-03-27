using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;
using Tera.WorldServer.World.Maps;

namespace Tera.WorldServer.World.Packets
{
    public sealed class ExchangeItemListMessage : PacketBase
    {
        public Npc Npc;

        public ExchangeItemListMessage(Npc Npc)
        {
            this.Npc = Npc;
        }


        public override string Compile()
        {
            var Packet = new StringBuilder("EL");

            this.Npc.SerializeAsItemList(Packet);

            return Packet.ToString();
        }
    }
}
