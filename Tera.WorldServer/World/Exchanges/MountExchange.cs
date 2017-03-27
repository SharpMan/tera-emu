using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Enumerations;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.Database.Tables;
using Tera.WorldServer.Network;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.World.Exchanges
{
    public class MountExchange : Exchange
    {
        private WorldClient myClient;

        public Mount Mount
        {
            get;
            set;
        }

        public MountExchange(WorldClient Client, Mount Mount)
        {
            this.myClient = Client;
            this.Mount = Mount;
            this.ExchangeType = (int)ExchangeTypeEnum.EXCHANGE_MOUNT;
        }

        public override bool MoveItem(Network.WorldClient Client, Database.Models.InventoryItemModel Item, ushort Quantity, bool Add = false)
        {
            if (Add)
            {
                if (Quantity > Item.Quantity)
                    Quantity = (ushort)Item.Quantity;

                InventoryItemModel ObjectEqualize = getSimilarItem(Item);
                int newQua = Item.Quantity - Quantity;
                String str;

                if (ObjectEqualize == null)
                {
                    if (newQua <= 0)
                    {
                        Client.Character.InventoryCache.RemoveItem(Item);
                        Mount.Items.Add(Item);
                        str = "O+" + Item.ID + "|" + Item.Quantity + "|" + Item.TemplateID + "|" + Item.GetStats().ToItemStats();
                        Client.Send(new ObjectRemoveMessage(Item.ID));
                    }
                    else
                    {
                        Client.Character.InventoryCache.UpdateObjectquantity(Item, newQua);
                        ObjectEqualize = InventoryItemTable.TryCreateItem(Item.TemplateID, Mount, Quantity, Stats: Item.Effects);
                        str = "O+" + ObjectEqualize.ID + "|" + ObjectEqualize.Quantity + "|" + ObjectEqualize.TemplateID + "|" + ObjectEqualize.GetStats().ToItemStats();
                    }
                }
                else if (newQua <= 0)
                {
                    Client.Character.InventoryCache.RemoveItem(Item);
                    ObjectEqualize.Quantity += Item.Quantity;
                    InventoryItemTable.removeItem(Item.ID);
                    str = "O+" + ObjectEqualize.ID + "|" + ObjectEqualize.Quantity + "|" + ObjectEqualize.TemplateID + "|" + ObjectEqualize.GetStats().ToItemStats();
                    Client.Send(new ObjectRemoveMessage(Item.ID));
                    InventoryItemTable.Update(ObjectEqualize);
                }
                else
                {
                    Client.Character.InventoryCache.UpdateObjectquantity(Item, newQua);
                    ObjectEqualize.Quantity += Quantity;
                    str = "O+" + ObjectEqualize.ID + "|" + ObjectEqualize.Quantity + "|" + ObjectEqualize.TemplateID + "|" + ObjectEqualize.GetStats().ToItemStats();
                    InventoryItemTable.Update(ObjectEqualize);
                }
                Client.Send(new BankUpdateMessage(str));
                Client.Send(new InventoryWeightMessage(0, 2000)); // TODO PODS
                Client.Send(new MountActualPodMessage(Mount));
                MountTable.Update(Mount);

            }
            else
            {
                if (!Mount.Items.Contains(Item))
                {
                    return false;
                }
                InventoryItemModel ObjectEqualize = Client.Character.InventoryCache.getSimilarInventoryItem(Item);
                int newQua = Item.Quantity - Quantity;
                String str;

                if (ObjectEqualize == null)
                {
                    if (newQua <= 0)
                    {
                        Mount.Items.Remove(Item);
                        if (Client.Character.InventoryCache.TryMergeItem(Item.TemplateID, Item.GetStats().ToItemStats(), Item.Slot, Item.Quantity))
                        {
                            InventoryItemTable.removeItem(Item.ID);
                        }
                        else
                        {
                            Client.Character.InventoryCache.Add(Item);
                        }
                        str = "O-" + Item.ID;
                    }
                    else
                    {
                        ObjectEqualize = InventoryItemTable.TryCreateItem(Item.TemplateID, Client.Character, Quantity, Stats: Item.Effects);
                        Item.Quantity = newQua;
                        str = "O+" + Item.ID + "|" + Item.Quantity + "|" +Item.TemplateID + "|" + Item.GetStats().ToItemStats();
                    }
                }
                else if (newQua <= 0)
                {
                    Mount.Items.Remove(Item);
                    ObjectEqualize.Quantity += Item.Quantity;
                    Client.Send(new ObjectQuantityMessage(ObjectEqualize.ID, ObjectEqualize.Quantity));
                    InventoryItemTable.removeItem(Item.ID);
                    str = "O-" + Item.ID;
                    InventoryItemTable.Update(ObjectEqualize);
                }
                else
                {
                    Item.Quantity = newQua;
                    ObjectEqualize.Quantity += Quantity;
                    Client.Send(new ObjectQuantityMessage(ObjectEqualize.ID, ObjectEqualize.Quantity));
                    str = "O+" + Item.ID + "|" + Item.Quantity + "|" + Item.TemplateID + "|" + Item.GetStats().ToItemStats();
                    InventoryItemTable.Update(ObjectEqualize);
                }
                Client.Send(new BankUpdateMessage(str));
                Client.Send(new InventoryWeightMessage(0, 2000)); // TODO PODS
                Client.Send(new MountActualPodMessage(Mount));
                MountTable.Update(Mount);
            }
            return true;
        }



        private InventoryItemModel getSimilarItem(InventoryItemModel item)
        {
            ItemTemplateModel objModel = item.Template;
            if (objModel.Type == 85 || objModel.Type == 18)
                return null;
            foreach (InventoryItemModel obj in Mount.Items)
            {
                if ((obj.Slot == ItemSlotEnum.SLOT_INVENTAIRE || obj.Position > 15) && item.ID != obj.ID && (obj.TemplateID == objModel.ID && (obj.GetStats().ToItemStats() == item.GetStats().ToItemStats() || obj.GetStats().ToItemStats().Equals(item.GetStats().ToItemStats()))))
                {
                    return obj;
                }
            }
            return null;
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
