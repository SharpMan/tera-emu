using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;
using Tera.WorldServer.Database.Models;

namespace Tera.WorldServer.World.Packets
{
    public sealed class ObjectAddInventoryMessage : PacketBase
    {
        public InventoryItemModel Item;

        public ObjectAddInventoryMessage(InventoryItemModel Item)
        {
            this.Item = Item;
        }

        public override string Compile()
        {
            return "OAKO" + this.Item.ToString();
        }
    }
}
