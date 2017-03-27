using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public sealed class ExchangeTaxItemListMessage : PacketBase
    {
        public String Content;

        public ExchangeTaxItemListMessage(String c)
        {
            Content = c;
        }

        public override string Compile()
        {
            return "EL" + Content;
        }

    }
}
