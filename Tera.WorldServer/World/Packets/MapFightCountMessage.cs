using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public sealed class MapFightCountMessage : PacketBase
    {
        public int FightCount;

        public MapFightCountMessage(int Count)
        {
            this.FightCount = Count;
        }

        public override string Compile()
        {
            return "fC" + this.FightCount;
        }
    }
}
