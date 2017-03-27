using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public sealed class ExchangeMoveDistantObject : PacketBase
    {
        public char Currencytype;
        public String Signature;
        public String Suplement;

        public ExchangeMoveDistantObject(char a, string b, string c)
        {
            this.Currencytype = a;
            this.Signature = b;
            this.Suplement = c;
        }

        public override string Compile()
        {
            String packet = "EmK" + Currencytype + Signature;
            if (!String.IsNullOrEmpty(Suplement))
                packet += Suplement;
            return packet;
        }
    }
}
