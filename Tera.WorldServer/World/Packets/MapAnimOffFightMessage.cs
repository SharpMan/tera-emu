using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public sealed class MapAnimOffFightMessage : PacketBase
    {
        public long guid;
        public int AnimType;

        public MapAnimOffFightMessage(long guid, int typeAnim)
        {
            this.guid = guid;
            this.AnimType = typeAnim;
        }

        public override string Compile()
        {
            return "GA0;228;" + this.guid + ";-1," + AnimType + ",12,0,0";
        }

    }
}
