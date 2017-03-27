using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public sealed class PingPacket : PacketBase
    {
        public Boolean Q;

        public PingPacket(bool Q)
        {
            this.Q = Q;
        }

        public override string Compile()
        {
            return Q ? "qpong" : "pong";
        }

    }
}
