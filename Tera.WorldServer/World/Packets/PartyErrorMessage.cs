using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public sealed class PartyErrorMessage : PacketBase
    {
        public string a;
        public PartyErrorMessage(string b)
        {
            this.a = b;
        }
        public override string Compile()
        {
            return "PIE" +a;
        }
    }
}
