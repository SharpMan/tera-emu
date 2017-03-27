using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.Database.Tables;
using Tera.WorldServer.Network;
using Tera.WorldServer.World.Maps;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.World.Exchanges
{
    public sealed class NpcExchange : Exchange
    {
        private WorldClient myClient;

        public Npc Npc
        {
            get;
            set;
        }

        public NpcExchange(WorldClient Client, Npc Npc)
        {
            this.myClient = Client;
            this.Npc = Npc;
        }

        public override bool MoveItem(WorldClient Client, InventoryItemModel Item, ushort Quantity, bool Add = false)
        {
            return false;
        }

        public override bool MoveKamas(WorldClient Client, long Quantity)
        {
            return false;
        }

        public override bool BuyItem(WorldClient Client, uint TemplateId, ushort Quantity)
        {
            if (this.myEnd) // ne devrait jamais arriver
                return false;

            if (!this.Npc.HasItemTemplate((int)TemplateId))
                return false;

            var Price = this.Npc.GetItemTemplate((int)TemplateId).Price;

            if (Price > Client.Character.Kamas)
                return false;

            if (InventoryItemTable.TryCreateItem((int)TemplateId, Client.Character, Quantity) == null)
                return false;

            Client.Character.InventoryCache.SubstractKamas(Price);
            Client.Send(new AccountStatsMessage(Client.Character));

            return true;
        }

        public override bool SellItem(WorldClient Client, InventoryItemModel Item, ushort Quantity)
        {
            if (this.myEnd) 
                return false;

            if (Quantity > Item.Quantity)
                Quantity = (ushort)Item.Quantity;

            uint Refund = (uint)Math.Floor((double)ItemTemplateTable.GetTemplate(Item.TemplateID).Price / 10) * Quantity;

            Client.Character.InventoryCache.AddKamas(Refund);
            if (Quantity == Item.Quantity)
            {
                //DatabaseEntities.TryDeleteItem(Item);
                InventoryItemTable.removeItem(Item.ID);
                Client.Send(new ObjectRemoveMessage(Item.ID));
            }
            else
            {
                Client.Character.InventoryCache.UpdateObjectquantity(Item, Item.Quantity - Quantity);
            }

            Client.Send(new AccountStatsMessage(Client.Character));

            return true;
        }

        public override bool Validate(WorldClient Client)
        {
            return false;
        }

        public override bool Finish()
        {
            this.myEnd = true;

            return true;
        }

        public override bool CloseExchange(bool Success = false)
        {
            this.Finish();

            this.myClient.SetExchange(null);
            this.myClient.Send(new ExchangeLeaveMessage(Success));

            return true;
        }

        public override void Send(PacketBase Packet)
        {
            this.myClient.Send(Packet);
        }
    }
}
