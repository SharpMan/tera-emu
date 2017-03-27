using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public sealed class ConquestPrismeAttackMessage : PacketBase
    {
        public String Context;

        public ConquestPrismeAttackMessage(string a)
        {
            this.Context = a;
        }

        public override string Compile()
        {
            return "CA" + Context;
        }
    }
}
