using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public sealed class GameRideMessage : PacketBase
    {
        public String Content;

        public GameRideMessage(string str)
        {
            this.Content = str;
        }

        public override string Compile()
        {
            return "R" + Content;
        }

    }
}
