using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public sealed class ConquestPrismSurvivedMessage : PacketBase
    {
        public String Context;

        public ConquestPrismSurvivedMessage(String s)
        {
            this.Context = s;
        }

        public override string Compile()
        {
            return "CS" + Context;
        }
    }
}
