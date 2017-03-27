using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public sealed class GuildTaxCollectorMessage : PacketBase
    {
        public String Content;

        public GuildTaxCollectorMessage(String content)
        {
            this.Content = content;
        }

        public override string Compile()
        {
            return "gT" + Content;
        }

    }
}
