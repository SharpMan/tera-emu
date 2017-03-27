using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public sealed class PartyLeaveMessage : PacketBase
    {
        public String s;

        public PartyLeaveMessage(String a)
        {
            s = a;
        }

        public override string Compile()
        {
            return "PV" + s;
        }
    }
}
