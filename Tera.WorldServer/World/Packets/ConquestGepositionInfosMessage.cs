using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public sealed class ConquestGepositionInfosMessage : PacketBase
    {
        public String Context;

        public ConquestGepositionInfosMessage(String s)
        {
            Context = s;
        }

        public override string Compile()
        {
            return "CW" + Context;
        }
    }
}
