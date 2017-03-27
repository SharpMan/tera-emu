using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public sealed class MountNameChangeMessage : PacketBase
    {
        public String Name;

        public MountNameChangeMessage(String name)
        {
            this.Name = name;
        }

        public override string Compile()
        {
            return "Rn" + Name;
        }

    }
}
