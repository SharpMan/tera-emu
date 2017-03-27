using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;
using Tera.WorldServer.Database.Models;

namespace Tera.WorldServer.World.Packets
{
    public sealed class MountDescriptionMessage : PacketBase
    {
        public Mount mount;

        public MountDescriptionMessage(Mount DD)
        {
            this.mount = DD;
        }

        public override string Compile()
        {
            return "Rd" + mount.parse();
        }

    }
}
