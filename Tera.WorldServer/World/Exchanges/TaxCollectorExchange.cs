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
    public sealed class TaxCollectorExchange : Exchange
    {
        private WorldClient myClient;

        public TaxCollector Npc
        {
            get;
            set;
        }

        public TaxCollectorExchange(WorldClient Client, TaxCollector Npc)
        {
            this.myClient = Client;
            this.Npc = Npc;
            this.ExchangeType = (int)ExchangeTypeEnum.EXCHANGE_TAXCOLLECTOR;
        }

        public override bool MoveItem(Network.WorldClient Client, Database.Models.InventoryItemModel Item, ushort Quantity, bool Add = false)
        {
            if (!Add)
            {
                if (!Npc.Items.ContainsKey(Item.ID))
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
                        Npc.Items.Remove(Item.ID);
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
                        str = "O+" + Item.ID + "|" + Item.Quantity + "|" + Item.TemplateID + "|" + Item.GetStats().ToItemStats();
                    }
                }
                else if (newQua <= 0)
                {
                    Npc.Items.Remove(Item.ID);
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
                TaxCollectorTable.Update(Npc);

                Npc.LogItems.Add(Item.ID, Item);
            }
            Client.GetCharacter().AddExperience(Npc.XP);
            Npc.LogXP += Npc.XP;
            Npc.XP = 0;
            TaxCollectorTable.Update(Npc);

            return true;
        }



        private InventoryItemModel getSimilarItem(InventoryItemModel item)
        {
            ItemTemplateModel objModel = item.Template;
            if (objModel.Type == 85 || objModel.Type == 18)
                return null;
            foreach (InventoryItemModel obj in Npc.Items.Values)
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
            if (Quantity > 0)
            {
                long P_Retrait = Npc.Kamas - Quantity;
                if (P_Retrait < 0)
                {
                    P_Retrait = 0;
                    Quantity = Npc.Kamas;
                }
                Npc.Kamas = P_Retrait;
                Client.GetCharacter().InventoryCache.AddKamas(Quantity);
                Client.Send(new AccountStatsMessage(Client.GetCharacter()));
                Client.Send(new BankUpdateMessage("G"+Npc.Kamas));
                return true;
            }
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
            Npc.Guild.Send(new GuildFightInformationsMesssage(Npc.Guild));
            Npc.Guild.Send(new GuildTaxCollectorMessage("G" + Npc.N1 + "," + Npc.N2 + "|.|" + Npc.Map.X + "|" + Npc.Map.Y + "|" + myClient.GetCharacter().Name + "|" + Npc.LogXP + ";" + Npc.GetLogItems()));
            Npc.Map.DestroyActor(Npc);
            TaxCollectorTable.TryDeleteTax(Npc);

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
