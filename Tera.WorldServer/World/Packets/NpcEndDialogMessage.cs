using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public class NpcEndDialogMessage : PacketBase
    {
        public override string Compile()
        {
            return "DV";
        }
    }
}
