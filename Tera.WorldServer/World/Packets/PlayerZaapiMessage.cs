using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public sealed class PlayerZaapiMessage : PacketBase
    {
        public short Mapid;
        public String List;

        public PlayerZaapiMessage(short a, string b)
        {
            this.Mapid = a;
            this.List = b;
        }

        public override string Compile()
        {
            return "Wc" + Mapid + "|" + List;
        }
    }
}
