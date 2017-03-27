using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;
using Tera.WorldServer.Database.Models;

namespace Tera.WorldServer.World.Packets
{
    public sealed class ExchangeShopItemListMessage : PacketBase
    {
        public ShopNpc Npc;

        public ExchangeShopItemListMessage(ShopNpc Npc)
        {
            this.Npc = Npc;
        }


        public override string Compile()
        {
            var Packet = new StringBuilder("EL");

            this.Npc.SerializeAsItemList(Packet);

            return Packet.ToString();
        }
    }
}
