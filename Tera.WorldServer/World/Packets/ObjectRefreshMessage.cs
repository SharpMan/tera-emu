using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;
using Tera.WorldServer.Database.Models;

namespace Tera.WorldServer.World.Packets
{
    public sealed class ObjectRefreshMessage : PacketBase
    {
        public InventoryItemModel Item;

        public ObjectRefreshMessage(InventoryItemModel I)
        {
            this.Item = I;
        }

        public override string Compile()
        {
            return "OCK" + Item.ToString();
        }
    }
}
