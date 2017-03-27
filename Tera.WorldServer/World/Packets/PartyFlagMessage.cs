using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public sealed class PartyFlagMessage : PacketBase
    {
        public String Content;

        public PartyFlagMessage(String s)
        {
            Content = s;
        }


        public override string Compile()
        {
            return "PF" + Content;
        }
    }
}
