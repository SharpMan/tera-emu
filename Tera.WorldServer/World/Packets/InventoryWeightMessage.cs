using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public sealed class InventoryWeightMessage : PacketBase
    {
        public int UsedPods;
        public int MaxPods;

        public InventoryWeightMessage(int UsedPods, int MaxPods)
        {
            this.UsedPods = UsedPods;
            this.MaxPods = MaxPods;
        }

        public override string Compile()
        {
            var Packet = new StringBuilder("Ow");

            Packet.Append(this.UsedPods);
            Packet.Append('|');
            Packet.Append(this.MaxPods);

            return Packet.ToString();
        }
    }
}
