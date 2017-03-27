using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public sealed class ObjectMoveSucessMessage : PacketBase
    {
        public long Guid;
        public short Position;

        public ObjectMoveSucessMessage(long Guid, short Pos)
        {
            this.Guid = Guid;
            this.Position = Pos;
        }

        public override string Compile()
        {
            return "OM" + this.Guid + '|' + (this.Position == -1 ? "" : this.Position.ToString());
        }
    }
}
