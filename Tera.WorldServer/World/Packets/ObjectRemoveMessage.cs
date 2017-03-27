using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public sealed class ObjectRemoveMessage : PacketBase
    {
        public long Guid;

        public ObjectRemoveMessage(long Guid)
        {
            this.Guid = Guid;
        }

        public override string Compile()
        {
            return "OR" + this.Guid;
        }
    }
}
