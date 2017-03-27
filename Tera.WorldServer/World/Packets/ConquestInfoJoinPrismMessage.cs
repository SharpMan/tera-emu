using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public sealed class ConquestInfoJoinPrismMessage : PacketBase
    {
        public String Context;

        public ConquestInfoJoinPrismMessage(String a)
        {
            this.Context = a;
        }


        public override string Compile()
        {
            return "CIJ" + Context;
        }
    }
}
