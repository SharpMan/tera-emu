using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.Network;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.World.Exchanges
{
    public class MarketBuyExchange : Exchange
    {
        private WorldClient myClient;

        public BidHouse Market
        {
            get;
            set;
        }

        public MarketBuyExchange(WorldClient Client, BidHouse BH)
        {
            this.myClient = Client;
            this.Market = BH;
            this.ExchangeType = (int)ExchangeTypeEnum.EXCHANGE_BIGSTORE_BUY;
        }

        public override bool MoveItem(Network.WorldClient Client, Database.Models.InventoryItemModel Item, ushort Quantity, bool Add = false)
        {
            return false;
        }

        public override bool MoveKamas(Network.WorldClient Client, long Quantity)
        {
            return false;
        }

        public override bool BuyItem(Network.WorldClient Client, uint TemplateId, ushort Quantity)
        {
            return false;
        }

        public override bool SellItem(Network.WorldClient Client, Database.Models.InventoryItemModel Item, ushort Quantity)
        {
            return false;
        }

        public override bool Validate(Network.WorldClient Client)
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

        public override void Send(Libs.Network.PacketBase Packet)
        {
            this.myClient.Send(Packet);
        }
    }
}
