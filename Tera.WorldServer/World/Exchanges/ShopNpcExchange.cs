using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.Database.Tables;
using Tera.WorldServer.Network;
using Tera.WorldServer.Utils;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.World.Exchanges
{
    public sealed class ShopNpcExchange : Exchange
    {
        private WorldClient myClient;

        public ShopNpc Npc
        {
            get;
            set;
        }

        public ShopNpcExchange(WorldClient Client, ShopNpc Npc)
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
            if (this.myEnd || Quantity <= 0 ) // ne devrait jamais arriver
                return false;

            if (!this.Npc.HasItemTemplate((int)TemplateId))
                return false;

            var Price = Settings.AppSettings.GetIntElement("NpcShop.Cost") * Quantity;
            var CachedP = Client.Character.Points;

            if (Price > CachedP)
            {
                Client.Send(new ChatGameMessage("Il vous manque " + (Price - CachedP) + " Points", "3882AC"));
                return false;
            }

            if (InventoryItemTable.TryCreateItem((int)TemplateId, Client.Character, Quantity,useMax : true) == null)
                return false;

            AccountTable.SubstractPoints(Client.Account.ID, Price);
            Client.Send(new ChatGameMessage("Il vous rêste désormais "+(CachedP - Price)+" Points","3882AC"));

            return true;
        }

        public override bool SellItem(WorldClient Client, InventoryItemModel Item, ushort Quantity)
        { 
           return false;
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
