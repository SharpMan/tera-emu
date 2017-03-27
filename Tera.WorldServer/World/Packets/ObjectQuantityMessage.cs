using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public sealed class ObjectQuantityMessage : PacketBase
    {
        public long ObjectGuid;
        public int Quantity;

        public ObjectQuantityMessage(long Guid, int Quantity)
        {
            this.ObjectGuid = Guid;
            this.Quantity = Quantity;
        }

        public override string Compile()
        {
            return "OQ" + this.ObjectGuid + '|' + this.Quantity;
        }
    }
}
