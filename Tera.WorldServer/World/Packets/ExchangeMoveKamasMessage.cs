using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public sealed class ExchangeMoveKamasMessage : PacketBase
    {
        public long Kamas;
        public bool Lower;

        public ExchangeMoveKamasMessage(long Kamas, bool Lower)
        {
            this.Kamas = Kamas;
            this.Lower = Lower;
        }

        public override string Compile()
        {
            return "E" + (Lower ? "m" : "M") + "KG" + this.Kamas;
        }
    }
}
