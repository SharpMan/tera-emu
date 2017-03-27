using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public sealed class ExchangeEndMessage : PacketBase
    {
        public char c;
        public String s;

        public ExchangeEndMessage(char c, String s)
        {
            this.c = c;
            this.s = s;
        }

        public override string Compile()
        {
            return "Ee" + c + s;
        }

    }
}
