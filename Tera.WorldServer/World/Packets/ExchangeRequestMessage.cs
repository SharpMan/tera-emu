using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public sealed class ExchangeRequestMessage : PacketBase
    {
        public long Trader;
        public long Target;

        public ExchangeRequestMessage(long Trader, long Target)
        {
            this.Trader = Trader;
            this.Target = Target;
        }

        public override string Compile()
        {
            return "ERK" + Trader + '|' + Target + "|1";
        }
    }
}
