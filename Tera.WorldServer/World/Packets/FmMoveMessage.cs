using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public sealed class FmMoveMessage : PacketBase
    {
        public String a;

        public FmMoveMessage(String b)
        {
            a = b;
        }

        public override string Compile()
        {
            return "Em" + a;
        }
    }
}
