using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public sealed class SpellUpdateFailMessage : PacketBase
    {
        public override string Compile()
        {
            return "SUE";
        }
    }
}
