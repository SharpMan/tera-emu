using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Tera.Libs;
using Tera.Libs.Enumerations;
using Tera.Libs.Helper;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.Database.Tables;
using Tera.WorldServer.Network;
using Tera.WorldServer.World.Controllers;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.Handlers
{
    class ObjectHandler
    {
        public static void ProcessPacket(WorldClient Client, string Packet)
        {
            switch (Packet[1])
            {
                case 'M': // Move
                    ObjectHandler.ProcessObjectMoveRequest(Client, Packet);
                    break;
                case 'u':
                case 'U': // Use
                    ObjectHandler.ProcessObjectUseRequest(Client, Packet);
                    break;

                case 'd': // Delete
                    ObjectHandler.ProcessObjectDestroyMessage(Client, Packet);
                    break;
                case 'f': // Eat
                    ObjectHandler.ProcessObjectEatLivingItemRequest(Client, Packet);
                    break;
                case 's': //Update Appearence
                    ObjectHandler.ProcessObjectAppareanceRequest(Client, Packet);
                    break;
                case 'x': //Dissocier
                    ObjectHandler.ProcessObjectUnEquipLivingItem(Client, Packet);
                    break;
            }
        }

        private static void ProcessObjectUnEquipLivingItem(WorldClient Client, string Packet)
        {
            if (Client.GetFight() != null && (int)Client.GetFight().FightState > 2)
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }
            var Data = Packet.Split('|');

            long ItemGuid = 0;

            if (!long.TryParse(Data[0].Substring(2), out ItemGuid))
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            var Item = Client.Character.InventoryCache.GetItem(ItemGuid);
            var OldId = Item.SpeakingID;

            if (Item == null || Item.SpeakingItem == null)
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            if (Item.SpeakingItem.Associated == 1)
            {
                long ObjeOriginialID = Item.SpeakingItem.LivingItem;
                var objv = InventoryItemTable.getItem(ObjeOriginialID);
                if (objv == null)
                {
                    Client.Send(new BasicNoOperationMessage());
                    return;
                }
                objv.SpeakingID = OldId;
                Item.SpeakingItem.LinkedItem = ObjeOriginialID;
                Item.SpeakingItem.Associated = 0;
                Item.SpeakingID = 0;
                Client.Character.InventoryCache.Add(objv);;
                Client.Send(new ObjectRefreshMessage(Item));
                Client.Character.myMap.SendToMap(new ObjectActualiseMessage(Client.Character));
                if (Client.GetFight() != null)
                    Client.GetFight().SendToFight(new ObjectActualiseMessage(Client.Character));
            }

        }

        private static void ProcessObjectAppareanceRequest(WorldClient Client, string Packet)
        {
            if (Client.GetFight() != null && (int)Client.GetFight().FightState > 2)
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }
            var Data = Packet.Split('|');

            long ItemGuid = 0;
            int aID = 0;

            if (Data.Length < 2 || !long.TryParse(Data[0].Substring(2), out ItemGuid))
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            var Item = Client.Character.InventoryCache.GetItem(ItemGuid);

            if (Item == null)
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            if (Item.SpeakingItem == null)
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            if (!int.TryParse(Data[2], out aID))
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            Item.SpeakingItem.Masque = aID;
            Client.Send(new ObjectRefreshMessage(Item));
            Client.Character.myMap.SendToMap(new ObjectActualiseMessage(Client.Character));
            if (Client.GetFight() != null)
                Client.GetFight().SendToFight(new ObjectActualiseMessage(Client.Character));
        }

        private static void ProcessObjectEatLivingItemRequest(WorldClient Client, string Packet)
        {
            if (Client.GetFight() != null && (int)Client.GetFight().FightState > 2)
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            var Data = Packet.Split('|');

            long ItemGuid = 0;
            long IdEatObject = 0;

            if (!long.TryParse(Data[0].Substring(2), out ItemGuid))
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            if (!long.TryParse(Data[2], out IdEatObject))
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            var Item = Client.Character.InventoryCache.GetItem(ItemGuid);
            var ItemAlim = Client.Character.InventoryCache.GetItem(IdEatObject);

            if (Item == null || ItemAlim == null)
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            if (Item.SpeakingItem == null)
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            if ((ItemAlim.Quantity > 1) && (Item.Quantity - 1 > 0))
            {
                int newQ = ItemAlim.Quantity - 1;
                InventoryItemTable.TryCreateItem(ItemAlim.TemplateID, Client.Character, Stats: ItemAlim.GetStats().ToItemStats(), quantity: newQ);
                Client.Character.InventoryCache.UpdateObjectquantity(ItemAlim, 1);
            }
            long xp = long.Parse(ItemAlim.Template.Level.ToString());
            Item.SpeakingItem.EXP += xp;
            Client.Character.InventoryCache.RemoveItem(ItemAlim);
            InventoryItemTable.removeItem(ItemAlim.ID);
            Client.Send(new ObjectRemoveMessage(ItemAlim.ID));
            Client.Send(new ObjectRefreshMessage(Item));
            Client.Character.myMap.SendToMap(new ObjectActualiseMessage(Client.Character));
            if (Client.GetFight() != null)
                Client.GetFight().SendToFight(new ObjectActualiseMessage(Client.Character));
            Client.Send(new AccountStatsMessage(Client.Character));

        }

        private static void ProcessObjectUseRequest(WorldClient Client, string Packet)
        {
            int guid = -1;
            int targetGuid = -1;
            int targetCell = -1;
            Player Target = null;
            try
            {
                String[] infos = Regex.Split(Packet.Substring(2),"\\|");
                guid = int.Parse(infos[0]);
                if (infos.Length == 3)
                {
                    targetCell = int.Parse(infos[2]);
                }
                try //try
                {
                    targetGuid = int.Parse(infos[1]);
                }
                catch (Exception e)
                {
                    targetGuid = -1;
                };
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return;
            }
            if (CharacterTable.GetCharacter(targetGuid) != null)
            {
                Target = CharacterTable.GetCharacter(targetGuid);
            }
            if (!Client.Character.InventoryCache.hasItemGuid(guid))
            {
                return;
            }

            InventoryItemModel obj = InventoryItemTable.getItem(guid);
            ItemTemplateModel T = obj.Template;
            if (!T.Criterions.Equals("") && !ConditionParser.validConditions(Client.Character, T.Criterions))
            {
                Client.Send(new ImMessage("119|43"));
                return;
            }
            if (T.ItemType == ItemTypeEnum.ITEM_TYPE_PAIN || T.ItemType == ItemTypeEnum.ITEM_TYPE_VIANDE_COMESTIBLE)
            {
                Client.Character.myMap.SendToMap(new MapEmoticoneMessage(Client.Character.ActorId, 17));
            }
            else if (T.ItemType == ItemTypeEnum.ITEM_TYPE_BIERE)
            {
                Client.Character.myMap.SendToMap(new MapEmoticoneMessage(Client.Character.ActorId, 18));
            }
            T.applyAction(Client.Character, Target, guid, (short)targetCell);
        }

        public static void ProcessObjectMoveRequest(WorldClient Client, string Packet)
        {
            if (Client.GetFight() != null && (int)Client.GetFight().FightState > 2)
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            var Data = Packet.Split('|');

            int ItemGuid = 0;
            int Position = 0;
            int Quantity = 1;

            if (!int.TryParse(Data[0].Substring(2), out ItemGuid))
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            if (!int.TryParse(Data[1], out Position))
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            if (Data.Length > 2)
            {
                int.TryParse(Data[2], out Quantity);
            }

            if (Position < (int)ItemSlotEnum.SLOT_INVENTAIRE || Position > (int)ItemSlotEnum.SLOT_ITEMBAR_14)
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            Client.Character.InventoryCache.MoveItem(ItemGuid, (ItemSlotEnum)Position, true, Quantity);
        }

        public static void ProcessObjectDestroyMessage(WorldClient Client, string Packet)
        {
            String[] infos = Regex.Split(Packet.Substring(2),"\\|");
            try
            {
                int guid = int.Parse(infos[0]);
                int qua = 1;
                try
                {
                    qua = int.Parse(infos[1]);
                }
                catch (Exception e)
                {
                    return;
                }

                InventoryItemModel obj = InventoryItemTable.getItem(guid);

                if (!Client.Character.InventoryCache.hasItemGuid(guid) || qua <= 0 || obj == null)
                {
                    Client.Send(new BasicNoOperationMessage());
                    return;
                }

                int newQua = obj.Quantity - qua;
                if (newQua <= 0)
                {
                    Client.Character.InventoryCache.remove(guid);
                    InventoryItemTable.removeItem(guid);
                    Client.Send(new ObjectRemoveMessage(guid));
                }
                else
                {
                    obj.Quantity = newQua;
                    Client.Send(new ObjectQuantityMessage(guid,newQua));
                }
                Client.Send(new AccountStatsMessage(Client.GetCharacter()));
                Client.Send(new InventoryWeightMessage(0, 2000));
            }
            catch (Exception e) { Client.Send(new BasicNoOperationMessage()); }
        }
    }
}
