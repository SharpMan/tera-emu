using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public sealed class BankUpdateMessage : PacketBase
    {
        public String Content;

        public BankUpdateMessage(String Content)
        {
            this.Content = Content;
        }

        public override string Compile()
        {
            return "EsK" + Content;
        }

    }
}
