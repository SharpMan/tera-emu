using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public sealed class MapEatOrDrinkMessage : PacketBase
    {
        public long guid;

        public MapEatOrDrinkMessage(long i)
        {
            guid = i;
        }

        public override string Compile()
        {
            return "eUK" + guid + "|17";
        }

    }
}
