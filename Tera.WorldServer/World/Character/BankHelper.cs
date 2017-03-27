using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.Libs.Enumerations;
using Tera.WorldServer.Database;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.Database.Tables;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.World.Character
{
    public static class BankHelper
    {
        public static void addInBank(Player character, int guid, int qua)
        {
            InventoryItemModel item = InventoryItemTable.getItem(guid);
            if(item == null)
            {
                character.Send(new BasicNoOperationMessage());
                return;
            }
            if (!character.InventoryCache.hasItemGuid(guid))
            {
                Logger.Error( "Le joueur " + character.Name + " a tenter d'ajouter un Item en banque qu'il n'avait pas.");
            }
            if(item.Slot != ItemSlotEnum.SLOT_INVENTAIRE)
            {
                character.Send(new BasicNoOperationMessage());
                return;
            }
            InventoryItemModel bankItem = getSimilarBankItem(character ,item);
            int newQua = item.Quantity - qua;
            if (bankItem == null)
            {
                if (newQua <= 0)
                {
                    character.InventoryCache.RemoveItem(item);
                    character.Client.Account.Data.bankItems.Add(item.ID, item);
                    String str = "O+" + item.ID + "|" + item.Quantity + "|" + item.TemplateID + "|" + item.GetStats().ToItemStats();
                    character.Send(new BankUpdateMessage(str));
                    character.Send(new ObjectRemoveMessage(guid));
                }

                else
                {
                    item.Quantity = newQua;
                    bankItem = getCloneItem(item, qua);
                    InventoryItemTable.addItem(bankItem, true);
                    character.Client.Account.Data.bankItems.Add(bankItem.ID, bankItem);
                    String str = "O+" + bankItem.ID + "|" + bankItem.Quantity + "|" + bankItem.TemplateID + "|" + bankItem.GetStats().ToItemStats();
                    character.Send(new BankUpdateMessage(str));
                    character.Send(new ObjectQuantityMessage(item.ID, item.Quantity));
                }
            }
            else
            {
                if (newQua <= 0)
                {
                    character.InventoryCache.RemoveItem(item);
                    InventoryItemTable.removeItem(item.ID);
                    bankItem.Quantity += item.Quantity;
                    String str = "O+" + bankItem.ID + "|" + bankItem.Quantity + "|" + bankItem.TemplateID + "|" + bankItem.GetStats().ToItemStats();
                    character.Send(new BankUpdateMessage(str));
                    character.Send(new ObjectRemoveMessage(guid));
                }
                else
                {
                    item.Quantity = newQua;
                    bankItem.Quantity += qua;
                    String str = "O+" + bankItem.ID + "|" + bankItem.Quantity + "|" + bankItem.TemplateID + "|" + bankItem.GetStats().ToItemStats();
                    character.Send(new BankUpdateMessage(str));
                    character.Send(new ObjectQuantityMessage(item.ID, item.Quantity));
                }
            }
            //SEND POD MESSAGE ?
            character.GetClient().Account.Data.Save();
        }




        public static void removeFromBank(Player character, int guid, int qua)
        {
            InventoryItemModel BankObj = InventoryItemTable.getItem(guid);
            if (BankObj == null)
            {
                character.Send(new BasicNoOperationMessage());
                return;
            }
            if (!character.Client.Account.Data.bankItems.ContainsKey(guid))
            {
                Logger.Error("Le joueur " + character.Name + " a tenter de retirer un Item en banque qu'il n'avait pas.");
            }
            InventoryItemModel PersoObj = character.InventoryCache.ItemsCache.Values.FirstOrDefault(x => x.TemplateID == BankObj.TemplateID && x.GetStats().ToItemStats() == BankObj.GetStats().ToItemStats() && x.ID != BankObj.ID && x.Position == -1); /* getSimilarBankItem(Character, BankObj);*/
            int newQua = BankObj.Quantity - qua;
            if (PersoObj == null)
            {
                if (newQua <= 0)
                {
                    character.Client.Account.Data.bankItems.Remove(guid);
                    character.InventoryCache.Add(BankObj,false);
                    String str = "O-" + guid;
                    character.Send(new BankUpdateMessage(str));
                }
                else
                {
                    PersoObj = getCloneItem(BankObj, qua);
                    InventoryItemTable.addItem(PersoObj, true);
                    character.InventoryCache.Add(PersoObj, false);
                    String str = "O+" + BankObj.ID + "|" + BankObj.Quantity + "|" + BankObj.TemplateID + "|" + BankObj.GetStats().ToItemStats();
                    character.Send(new BankUpdateMessage(str));
                }
            }
            else
            {
                if (newQua <= 0)
                {
                    character.Client.Account.Data.bankItems.Remove(BankObj.ID);
                    InventoryItemTable.removeItem(BankObj.ID);
                    PersoObj.Quantity += BankObj.Quantity;
                    character.Send(new ObjectQuantityMessage(PersoObj.ID, PersoObj.Quantity));
                    String str = "O-" + guid;
                    character.Send(new BankUpdateMessage(str));
                }
                else
                {
                    BankObj.Quantity = newQua;
                    PersoObj.Quantity += qua;
                    character.Send(new ObjectQuantityMessage(PersoObj.ID, PersoObj.Quantity));
                    String str = "O+" + BankObj.ID + "|" + BankObj.Quantity + "|" + BankObj.TemplateID + "|" + BankObj.GetStats().ToItemStats();
                    character.Send(new BankUpdateMessage(str));
                }
            }
            //SEND POD MESSAGE ?
            character.GetClient().Account.Data.Save();
        }

        private static InventoryItemModel getSimilarBankItem(Player character, InventoryItemModel obj)
        {
            foreach (InventoryItemModel value in character.Client.Account.Data.bankItems.Values.Where(x => x.Template.ID == obj.Template.ID))
            {
                if (value.Template.ItemType == ItemTypeEnum.ITEM_TYPE_PIERRE_AME_PLEINE)
                {
                    continue;
                }
                if (value.GetStats().ToItemStats() == obj.GetStats().ToItemStats())
                {
                    return value;
                }
            }
            return null;
        }

        public static InventoryItemModel getCloneItem(InventoryItemModel obj, int qua)
        {
            InventoryItemModel ob = new InventoryItemModel()
            {
                ID = DatabaseCache.nextItemGuid++,
                TemplateID = obj.TemplateID,
                Quantity = qua,
                Position = -1,
                Effects = obj.GetStats().ToItemStats(),
            };
            return ob;
        }
    }
}
