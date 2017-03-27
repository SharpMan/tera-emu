using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public sealed class SubAreaAlignMessage : PacketBase
    {
        //Je pense pas ont dois devenir plusieurs var a chaque fois il y'a une new forme de Packet

        public String Content;

        public SubAreaAlignMessage(String c)
        {
            this.Content = c;
        }

        public override string Compile()
        {
            return "am" + Content;
        }
    }
}
