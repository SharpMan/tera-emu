using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public sealed class GameActionFightMessage : PacketBase
    {
        public int type;
        public long guid;

        public GameActionFightMessage(int type, long guid)
        {
            this.type = type;
            this.guid = guid;
        }

        public override string Compile()
        {
            return "GAF" + type + "|" + guid;
        }

    }
}
