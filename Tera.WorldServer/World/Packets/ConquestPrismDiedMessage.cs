using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public sealed class ConquestPrismDiedMessage : PacketBase
    {
        public String Context;

        public ConquestPrismDiedMessage(String s)
        {
            this.Context = s;
        }

        public override string Compile()
        {
            return "CD" + Context;
        }
    }
}
