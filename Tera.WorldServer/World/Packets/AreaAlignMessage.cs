using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public sealed class AreaAlignMessage : PacketBase
    {

        public String Content;

        public AreaAlignMessage(String c)
        {
            this.Content = c;
        }

        public override string Compile()
        {
            return "aM" + Content;
        }
    }
}
