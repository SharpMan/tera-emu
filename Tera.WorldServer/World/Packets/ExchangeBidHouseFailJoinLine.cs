using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public sealed class ExchangeBidHouseFailJoinLine : PacketBase
    {
        public String Signature, Str;

        public ExchangeBidHouseFailJoinLine(string s, string r)
        {
            this.Signature = s;
            this.Str = r;
        }

        public override string Compile()
        {
            return "EHM" + Signature + Str;
        }
    }
}
