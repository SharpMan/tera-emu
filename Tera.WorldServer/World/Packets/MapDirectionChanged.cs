using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public sealed class MapDirectionChanged : PacketBase
    {
        public long guid;
        public int dir;

        public MapDirectionChanged(long guid, int dir)
        {
            this.guid = guid;
            this.dir = dir;
        }

        public override string Compile()
        {
            return "eD" + guid + "|" + dir;
        }


    }
}
