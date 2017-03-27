using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;
using Tera.WorldServer.Database.Models;

namespace Tera.WorldServer.World.Packets
{
    public class MountActualPodMessage : PacketBase
    {
        public Mount Mount;

        public MountActualPodMessage(Mount Mount)
        {
            this.Mount = Mount;
        }


        public override string Compile()
        {
            return "Ew" + Mount.getActualPods() + ";1000";
        }
    }
}
