using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.WorldServer.Network;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.World.Packets;
using Tera.WorldServer.Database.Tables;

namespace Tera.WorldServer.World.Exchanges
{
    public class MarketSellExchange : Exchange
    {
        private WorldClient myClient;

        public BidHouse Market
        {
            get;
            set;
        }

        public long Price
        {
            get;
            set;
        }

        public long ItemID
        {
            get;
            set;
        }

        public MarketSellExchange(WorldClient Client, BidHouse BH)
        {
            this.myClient = Client;
            this.Market = BH;
            this.ExchangeType = (int)ExchangeTypeEnum.EXCHANGE_BIGSTORE_SELL;
        }

        public override bool MoveItem(WorldClient Client, Database.Models.InventoryItemModel Item, ushort Quantity, bool Add = false)
        {
            if (Add)
            {
                long Taxe = (long)(Price * (Market.SellTaxe / 100));
                if (myClient.Account.Data.canTaxBidHouseItem(Market.HdvID) >= Market.countItem)
                {
                    myClient.Send(new TextInformationMessage(Libs.Enumerations.TextInformationTypeEnum.INFO, 58));
                    return false;
                }
                if (myClient.Character.Kamas < Taxe)
                {
                    myClient.Send(new TextInformationMessage(Libs.Enumerations.TextInformationTypeEnum.ERREUR, 76));
                    return false;
                }
                myClient.Character.InventoryCache.SubstractKamas(Taxe);
                int cantReal = (int)(Math.Pow(10, Quantity) / 10);
                int nuevaCant = Item.Quantity - cantReal;
                if (nuevaCant <= 0)
                {
                    myClient.Character.InventoryCache.RemoveItem(Item);
                    myClient.Character.Send(new ObjectRemoveMessage(Item.ID));
                }
                else
                {
                    Item.Quantity -= cantReal;
                    myClient.Send(new ObjectQuantityMessage(Item.ID, Item.Quantity));
                    Item = InventoryItemTable.TryCreateItem(Item.TemplateID, quantity: cantReal, Stats: Item.GetStats().ToItemStats());
                }
                var MarketItem = new BidHouseItem()
                {
                    Price = Price,
                    Quantity = Quantity,
                    Owner = myClient.Account.ID,
                    Item = Item,
                };
                Market.addObject(MarketItem);
                BidHouseTable.Add(MarketItem);
                myClient.Send(new ExchangeMoveDistantObject('+', "", MarketItem.SerializeAsDisplayEquipmentOnMarket()));
                Client.Send(new AccountStatsMessage(myClient.Character));
            }
            else
            {
                var MarketID = Market.HdvID;
                BidHouseItem BHI = null;
                try
                {
                    foreach (var BH in myClient.Account.Data.BHI[MarketID])
                    {
                        if (BH.LineID == ItemID)
                        {
                            BHI = BH;
                            break;
                        }
                    }
                }
                catch (Exception e)
                {
                    return false;
                }
                if (BHI == null)
                    return false;
                myClient.Account.Data.BHI[MarketID].Remove(BHI);
                var Object = BHI.Item;
                myClient.Character.InventoryCache.Add(Object);
                Market.DestroyObject(BHI);
                BidHouseTable.Delete(BHI.ItemID);
                myClient.Send(new ExchangeMoveDistantObject('-', "", ItemID + ""));
            }
            return true;
        }

        public override bool MoveKamas(WorldClient Client, long Quantity)
        {
            return false;
        }

        public override bool BuyItem(WorldClient Client, uint TemplateId, ushort Quantity)
        {
            return false;
        }

        public override bool SellItem(WorldClient Client, Database.Models.InventoryItemModel Item, ushort Quantity)
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

        public override void Send(Libs.Network.PacketBase Packet)
        {
            this.myClient.Send(Packet);
        }
    }
}
