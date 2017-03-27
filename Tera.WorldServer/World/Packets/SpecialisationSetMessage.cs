using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public sealed class SpecialisationSetMessage : PacketBase
    {
        public int Alignment;

        public SpecialisationSetMessage(int Alignment)
        {
            this.Alignment = Alignment;
        }

        public override string Compile()
        {
            return "ZS" + this.Alignment;
        }
    }
}
