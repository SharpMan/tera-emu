using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Tera.Libs;
using Tera.Libs.Network;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.Database.Tables;
using Tera.WorldServer.Network;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.World.Exchanges
{
    public sealed class PlayerExchange : Exchange
    {
        private WorldClient myClient1, myClient2;
        private Dictionary<WorldClient, Dictionary<long, ushort>> myItemsToTrade = new Dictionary<WorldClient, Dictionary<long, ushort>>();
        private Dictionary<WorldClient, long> myKamasToTrade = new Dictionary<WorldClient, long>();
        private Dictionary<WorldClient, bool> myValidate = new Dictionary<WorldClient, bool>();

        public PlayerExchange(WorldClient Client1, WorldClient Client2)
        {
            this.myItemsToTrade.Add(Client1, new Dictionary<long, ushort>());
            this.myItemsToTrade.Add(Client2, new Dictionary<long, ushort>());
            this.myKamasToTrade.Add(Client1, 0);
            this.myKamasToTrade.Add(Client2, 0);
            this.myValidate.Add(Client1, false);
            this.myValidate.Add(Client2, false);

            this.myClient1 = Client1;
            this.myClient2 = Client2;

            Logger.Debug("PlayerExchange launched : Player1=" + this.myClient1.Account.Username + " Player2=" + this.myClient2.Account.Username);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public override bool MoveItem(WorldClient Client, InventoryItemModel Item, ushort Quantity, bool Add = false)
        {
            this.UnValidateAll();

            if (Add)
            {
                if (Quantity > Item.Quantity)
                    Quantity = (ushort)Item.Quantity;

                if (this.myItemsToTrade[Client].ContainsKey(Item.ID))
                {
                    if (this.myItemsToTrade[Client][Item.ID] == Item.Quantity)
                        return false; 
                    else if (this.myItemsToTrade[Client][Item.ID] + Quantity > Item.Quantity)
                        Quantity = (ushort)(Item.Quantity - this.myItemsToTrade[Client][Item.ID]); 

                    this.myItemsToTrade[Client][Item.ID] += Quantity;
                }
                else
                {
                    this.myItemsToTrade[Client].Add(Item.ID, Quantity);
                }

                if (Client == this.myClient1)
                {
                    this.myClient1.Send(new ExchangeMoveSucessMessage(ExchangeMoveType.TYPE_OBJECT, Item.ID + "|" + this.myItemsToTrade[Client][Item.ID], false, true));
                    this.myClient2.Send(new ExchangeMoveSucessMessage(ExchangeMoveType.TYPE_OBJECT, Item.ID + "|" + this.myItemsToTrade[Client][Item.ID] + "|" + Item.TemplateID + "|" + Item.GetStats().ToItemStats(), true, true));
                }
                else
                {
                    this.myClient2.Send(new ExchangeMoveSucessMessage(ExchangeMoveType.TYPE_OBJECT, Item.ID + "|" + this.myItemsToTrade[Client][Item.ID], false, true));
                    this.myClient1.Send(new ExchangeMoveSucessMessage(ExchangeMoveType.TYPE_OBJECT, Item.ID + "|" + this.myItemsToTrade[Client][Item.ID] + "|" + Item.TemplateID + "|" + Item.GetStats().ToItemStats(), true, true));
                }
            }
            else
            {
                if (!this.myItemsToTrade[Client].ContainsKey(Item.ID))
                    return false;

                if (Quantity >= this.myItemsToTrade[Client][Item.ID])
                {
                    this.myItemsToTrade[Client].Remove(Item.ID);

                    if (Client == this.myClient1)
                    {
                        this.myClient1.Send(new ExchangeMoveSucessMessage(ExchangeMoveType.TYPE_OBJECT, Item.ID.ToString(), false, false));
                        this.myClient2.Send(new ExchangeMoveSucessMessage(ExchangeMoveType.TYPE_OBJECT, Item.ID.ToString(), true, false));
                    }
                    else
                    {
                        this.myClient2.Send(new ExchangeMoveSucessMessage(ExchangeMoveType.TYPE_OBJECT, Item.ID.ToString(), false, false));
                        this.myClient1.Send(new ExchangeMoveSucessMessage(ExchangeMoveType.TYPE_OBJECT, Item.ID.ToString(), true, false));
                    }
                }
                else
                {
                    this.myItemsToTrade[Client][Item.ID] -= Quantity;

                    if (Client == this.myClient1)
                    {
                        this.myClient1.Send(new ExchangeMoveSucessMessage(ExchangeMoveType.TYPE_OBJECT, Item.ID + "|" + this.myItemsToTrade[Client][Item.ID], false, true));
                        this.myClient2.Send(new ExchangeMoveSucessMessage(ExchangeMoveType.TYPE_OBJECT, Item.ID + "|" + this.myItemsToTrade[Client][Item.ID] + "|" + Item.TemplateID + "|" + Item.GetStats().ToItemStats(), true, true));
                    }
                    else
                    {
                        this.myClient2.Send(new ExchangeMoveSucessMessage(ExchangeMoveType.TYPE_OBJECT, Item.ID + "|" + this.myItemsToTrade[Client][Item.ID], false, true));
                        this.myClient1.Send(new ExchangeMoveSucessMessage(ExchangeMoveType.TYPE_OBJECT, Item.ID + "|" + this.myItemsToTrade[Client][Item.ID] + "|" + Item.TemplateID + "|" + Item.GetStats().ToItemStats(), true, true));
                    }
                }
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public override bool MoveKamas(WorldClient Client, long Quantity)
        {
            Logger.Debug("PlayerExchange(" + this.myClient1.Account.Username + " - " + this.myClient2.Account.Username + ")::MoveKamas : Player=" + Client.Character.Name);

            this.UnValidateAll();

            if (Quantity > Client.Character.Kamas)
                Quantity = Client.Character.Kamas;

            this.myKamasToTrade[Client] = Quantity;

            if (Client == this.myClient1)
            {
                this.myClient1.Send(new ExchangeMoveKamasMessage(Quantity, false));
                this.myClient2.Send(new ExchangeMoveKamasMessage(Quantity, true));
            }
            else
            {
                this.myClient2.Send(new ExchangeMoveKamasMessage(Quantity, false));
                this.myClient1.Send(new ExchangeMoveKamasMessage(Quantity, true));
            }

            return true;
        }

        public override bool BuyItem(WorldClient Client, uint TemplateID, ushort Quantity)
        {
            return false;
        }

        public override bool SellItem(WorldClient Client, InventoryItemModel Item, ushort Quantity)
        {
            return false;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UnValidateAll()
        {
            this.myValidate[this.myClient1] = false;
            this.myValidate[this.myClient2] = false;

            this.Send(new ExchangeValidateMessage(this.myClient1.Character.ID, false));
            this.Send(new ExchangeValidateMessage(this.myClient2.Character.ID, false));
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public override bool Validate(WorldClient Client)
        {
            this.myValidate[Client] = this.myValidate[Client] == false;

            this.Send(new ExchangeValidateMessage(Client.Character.ID, this.myValidate[Client]));

            if (this.myValidate.All(Validate => Validate.Value))
            {
                this.Finish();

                this.myClient1.EndGameAction(GameActionTypeEnum.EXCHANGE);

                this.Dispose();

                return true;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public override bool Finish()
        {
            if (this.myEnd)
                return false;

            Logger.Debug("PlayerExchange(" + this.myClient1.Account.Username + " - " + this.myClient2.Account.Username + ")::Finish()" +
               "\n          -- P1(Items=" + string.Join(",", this.myItemsToTrade[this.myClient1].Select(x => x.Key)) + " Kamas=" + this.myKamasToTrade[this.myClient1] + ")" +
               "\n          -- P2(Items=" + string.Join(",", this.myItemsToTrade[this.myClient2].Select(x => x.Key)) + " Kamas=" + this.myKamasToTrade[this.myClient2] + ")");


            foreach (var ItemData in this.myItemsToTrade[this.myClient1])
            {
                var Item = this.myClient1.Character.InventoryCache.GetItem(ItemData.Key);
                Item.Quantity -= ItemData.Value; 

                if (Item.Quantity == 0)
                {
                    InventoryItemTable.removeItem(Item.ID);
                    this.myClient1.Character.InventoryCache.RemoveItem(Item);
                    this.myClient1.Send(new ObjectRemoveMessage(Item.ID));
                }
                else if (Item.Quantity < 0)
                {
                    Logger.Debug("PlayerExchange::Finish() item quantity < 0 : " + myClient1.Character.Name);
                    InventoryItemTable.removeItem(Item.ID);
                    this.myClient1.Character.InventoryCache.RemoveItem(Item);
                }
                else
                    this.myClient1.Character.InventoryCache.UpdateObjectquantity(Item, Item.Quantity);

                InventoryItemTable.TryCreateItem(Item.TemplateID, this.myClient2.Character, ItemData.Value, Stats: Item.Effects);
            }

            foreach (var ItemData in this.myItemsToTrade[this.myClient2])
            {
                var Item = this.myClient2.Character.InventoryCache.GetItem(ItemData.Key);
                Item.Quantity -= ItemData.Value; 

                if (Item.Quantity == 0)
                {
                    InventoryItemTable.removeItem(Item.ID);
                    this.myClient2.Character.InventoryCache.RemoveItem(Item);
                    this.myClient2.Send(new ObjectRemoveMessage(Item.ID));
                }
                else if (Item.Quantity < 0)
                {
                    Logger.Debug("PlayerExchange::Finish() item quantity < 0 : " + myClient2.Character.Name);
                    InventoryItemTable.removeItem(Item.ID);
                    this.myClient2.Character.InventoryCache.RemoveItem(Item);
                }
                else
                    this.myClient2.Character.InventoryCache.UpdateObjectquantity(Item, Item.Quantity);

                InventoryItemTable.TryCreateItem(Item.TemplateID, this.myClient1.Character, ItemData.Value, Stats: Item.Effects);
            }

            this.myClient1.Character.InventoryCache.SubstractKamas(this.myKamasToTrade[this.myClient1]);
            this.myClient2.Character.InventoryCache.SubstractKamas(this.myKamasToTrade[this.myClient2]);

            this.myClient1.Character.InventoryCache.AddKamas(this.myKamasToTrade[this.myClient2]);
            this.myClient2.Character.InventoryCache.AddKamas(this.myKamasToTrade[this.myClient1]);

            this.myClient1.Send(new AccountStatsMessage(myClient1.Character));
            this.myClient2.Send(new AccountStatsMessage(myClient2.Character));

            return true;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public override bool CloseExchange(bool Success = false)
        {
            this.myClient1.SetExchange(null);
            this.myClient2.SetExchange(null);

            var Message = new ExchangeLeaveMessage(Success);

            this.Send(Message);

            if (!Success)
                this.Dispose();

            this.myEnd = true;

            return true;
        }

        public void Dispose()
        {
            this.myItemsToTrade.Clear();
            this.myKamasToTrade.Clear();
            this.myValidate.Clear();
            this.myClient1 = null;
            this.myClient2 = null;
        }

        public override void Send(PacketBase Packet)
        {
            this.myClient1.Send(Packet);
            this.myClient2.Send(Packet);
        }
    }
}
