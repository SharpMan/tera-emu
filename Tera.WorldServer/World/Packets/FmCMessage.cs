using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public sealed class FmCMessage : PacketBase
    {
        public string a;
        public FmCMessage(String b)
        {
            a = b;
        }
        public override string Compile()
        {
            return "Ec" + a;
        }
    }
}
