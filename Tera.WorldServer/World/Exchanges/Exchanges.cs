using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.Network;

namespace Tera.WorldServer.World.Exchanges
{
    public enum ExchangeTypeEnum
    {
        EXCHANGE_SHOP = 0, 
        EXCHANGE_PLAYER = 1,
        EXCHANGE_STORAGE = 5,
        EXCHANGE_TAXCOLLECTOR = 8,
        EXCHANGE_PERSONAL_SHOP_EDIT = 6,
        EXCHANGE_BIGSTORE_SELL = 10,
        EXCHANGE_BIGSTORE_BUY = 11,
        EXCHANGE_MOUNT_STORAGE = 16,
        EXCHANGE_MOUNT = 15
    }

    public abstract class Exchange
    {
        protected bool myEnd = false;

        public bool ExchangeFinish { get { return this.myEnd; } }
        public int ExchangeType { get; set; }

        public abstract bool MoveItem(WorldClient Client, InventoryItemModel Item, ushort Quantity, bool Add = false);
        public abstract bool MoveKamas(WorldClient Client, long Quantity);


        public abstract bool BuyItem(WorldClient Client, uint TemplateId, ushort Quantity);
        public abstract bool SellItem(WorldClient Client, InventoryItemModel Item, ushort Quantity);

        public abstract bool Validate(WorldClient Client);
        public abstract bool Finish();

        public abstract bool CloseExchange(bool Success = false);

        public abstract void Send(PacketBase Packet);
    }
}
