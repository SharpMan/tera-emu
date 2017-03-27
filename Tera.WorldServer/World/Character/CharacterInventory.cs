using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Tera.Libs;
using Tera.Libs.Enumerations;
using Tera.WorldServer.Database;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.Database.Tables;
using Tera.WorldServer.World.Controllers;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.World.Character
{
    public class CharacterInventory
    {
        private Player Player;
        public Dictionary<long, InventoryItemModel> ItemsCache = new Dictionary<long, InventoryItemModel>();
        public Dictionary<int, GenericStats> ItemSetEffects = new Dictionary<int, GenericStats>();

        public CharacterInventory(Player character)
        {
            this.Player = character;
            if (!String.IsNullOrEmpty(character.Stuff))
            {
                if (character.Stuff[character.Stuff.Length - 1] == '|')
                {
                    character.Stuff = character.Stuff.Substring(0, character.Stuff.Length - 1);
                }
                InventoryItemTable.Load(character.Stuff.Replace("|", ","));
            }
            foreach (String item in Regex.Split(character.Stuff, "\\|"))
            {
                if (String.IsNullOrEmpty(item))
                {
                    continue;
                }
                int guid = int.Parse(item);

                InventoryItemModel obj = InventoryItemTable.getItem(guid);
                if (obj != null)
                {
                    Add(obj);
                }
            }
        }

        public bool Add(InventoryItemModel item, bool merge = true)
        {
            if (merge && TryMergeItem(item.TemplateID, item.Effects, item.Slot, item.Quantity))
                return false;
            if (ItemsCache.ContainsKey(item.ID))
            {
                ItemsCache.Remove(item.ID);
            }
            ItemsCache.Add(item.ID, item);
            Player.Send(new ObjectAddInventoryMessage(item));
            return true;
        }

        public InventoryItemModel getSimilarInventoryItem(InventoryItemModel item)
        {
            if (item.Template.Type == 85)
                return null;
            else
            {
                foreach (InventoryItemModel obj in ItemsCache.Values)
                {
                    if ((obj.Position == -1 || obj.Position > 15) && item.ID != obj.ID && (obj.TemplateID == item.TemplateID) && obj.GetStats().ToItemStats() == item.GetStats().ToItemStats())
                    {
                        return obj;
                    }
                }
            }
            return null;
        }

        public InventoryItemModel get(int guid)
        {
            if (!ItemsCache.ContainsKey(guid))
            {
                return null;
            }
            return ItemsCache[guid];
        }

        public void remove(long guid)
        {
            if (ItemsCache.ContainsKey(guid))
            {
                ItemsCache.Remove(guid);
            }
        }

        public Boolean hasItemGuid(int guid)
        {
            if (!ItemsCache.ContainsKey(guid))
            {
                return false;
            }
            return ItemsCache[guid] != null ? ItemsCache[guid].Quantity > 0 : false;
        }

        public void removeByTemplateID(int tID, int count)
        {
            List<InventoryItemModel> list = this.ItemsCache.Values.Where(x => x.TemplateID == tID).ToList();
            List<InventoryItemModel> remove = new List<InventoryItemModel>();

            int tempCount = count;

            foreach (InventoryItemModel obj in list)
            {
                if (obj.Quantity >= count)
                {
                    int newQua = obj.Quantity - count;
                    if (newQua > 0)
                    {
                        obj.Quantity = newQua;
                        if (this.Player.IsOnline())
                        {
                            Player.Send(new ObjectQuantityMessage(obj.ID, obj.Quantity));
                        }
                    }
                    else
                    {
                        this.ItemsCache.Remove(obj.ID);
                        InventoryItemTable.removeItem(obj.ID);

                        if (this.Player.IsOnline())
                        {
                            Player.Send(new ObjectRemoveMessage(obj.ID));
                        }
                    }
                    return;
                }
                if (obj.Quantity >= tempCount)
                {
                    int newQua = obj.Quantity - tempCount;
                    if (newQua > 0)
                    {
                        obj.Quantity = newQua;
                        if (this.Player.IsOnline())
                        {
                            Player.Send(new ObjectQuantityMessage(obj.ID, obj.Quantity));
                        }
                    }
                    else
                    {
                        remove.Add(obj);
                    }
                    foreach (InventoryItemModel o in remove)
                    {
                        this.ItemsCache.Remove(o.ID);
                        InventoryItemTable.removeItem(o.ID);

                        if (this.Player.IsOnline())
                        {
                            Player.Send(new ObjectRemoveMessage(obj.ID));
                        }
                    }
                }
                else
                {
                    tempCount -= obj.Quantity;
                    remove.Add(obj);
                }
            }
        }

        public InventoryItemModel GetItemInSlot(ItemSlotEnum Slot)
        {
            return ItemsCache.Values.FirstOrDefault(x => x.Slot == Slot);
            /*foreach (var item in this.Items)
                if (item.Slot == Slot)
                    return item;

            return null;*/
        }


        public bool TryMergeItem(int TemplateId, string Stats, ItemSlotEnum Slot, int Quantity = 1, InventoryItemModel RemoveItem = null)
        {
            if (Slot < ItemSlotEnum.SLOT_AMULETTE || Slot > ItemSlotEnum.SLOT_BOUCLIER)
            {
                var ItemsCopy = this.ItemsCache.ToArray();

                foreach (var Item in ItemsCopy)
                    if (Item.Value.GetStats().ToItemStats() == Stats && Item.Value.TemplateID == TemplateId && Item.Value.Slot == Slot)
                    {
                        if (RemoveItem != null)
                        {
                            InventoryItemTable.removeItem(RemoveItem.ID);
                            Player.Send(new ObjectRemoveMessage(RemoveItem.ID));
                        }

                        this.UpdateObjectquantity(Item.Value, Item.Value.Quantity + Quantity);
                        return true;
                    }
            }

            return false;
        }

        public void onMoveItem(InventoryItemModel Item)
        {
            lock (this.ItemSetEffects)
            {
                if (Item.Template.ItemSetID != -1)
                {
                    if (ItemSetTable.getItemSet(Item.Template.ItemSetID) == null)
                        return;
                    var countByItemSet = CountItemByItemSet(Item.Template.ItemSetID);
                    var Stat = ItemSetTable.getItemSet(Item.Template.ItemSetID).getBonusStatByItemCount(countByItemSet);
                    if (!this.ItemSetEffects.Keys.Contains(Item.Template.ItemSetID))
                    {
                        if (countByItemSet > 1)
                        {
                            this.ItemSetEffects.Add(Item.Template.ItemSetID, Stat);
                            this.Player.myStats.Merge(Stat);
                            this.Player.Life += Stat.GetTotal(EffectEnum.AddVitalite);
                        }
                    }
                    else
                    {
                        if (countByItemSet <= 1)
                        {
                            this.Player.myStats.UnMerge(this.ItemSetEffects[Item.Template.ItemSetID]);
                            this.Player.Life -= this.ItemSetEffects[Item.Template.ItemSetID].GetTotal(EffectEnum.AddVitalite);
                            this.ItemSetEffects.Remove(Item.Template.ItemSetID);
                        }
                        else if (this.ItemSetEffects[Item.Template.ItemSetID] != Stat)
                        {
                            this.Player.myStats.UnMerge(this.ItemSetEffects[Item.Template.ItemSetID]);
                            this.Player.Life -= this.ItemSetEffects[Item.Template.ItemSetID].GetTotal(EffectEnum.AddVitalite);
                            this.ItemSetEffects[Item.Template.ItemSetID] = Stat;
                            this.Player.myStats.Merge(Stat);
                            this.Player.Life += Stat.GetTotal(EffectEnum.AddVitalite);
                        }
                    }
                    Player.BeginCachedBuffer();
                    Player.Send(new ItemSetAppearMessage(Player, Item.Template.ItemSetID));
                    Player.Send(new AccountStatsMessage(Player));
                    Player.EndCachedBuffer();

                }
            }
        }

        public void RefreshSet()
        {
            lock (ItemsCache)
            {
                foreach (var Item in this.ItemsCache.Values.Where(x => x != null && x.Template.ItemSetID != -1 && ItemSetTable.getItemSet(x.Template.ItemSetID) != null && x.Slot > ItemSlotEnum.SLOT_INVENTAIRE && x.Slot <= ItemSlotEnum.SLOT_BOUCLIER))
                {
                    if (!this.ItemSetEffects.Keys.Contains(Item.Template.ItemSetID))
                    {
                        if (CountItemByItemSet(Item.Template.ItemSetID) > 1)
                        {
                            this.ItemSetEffects.Add(Item.Template.ItemSetID, ItemSetTable.getItemSet(Item.Template.ItemSetID).getBonusStatByItemCount(CountItemByItemSet(Item.Template.ItemSetID)));
                            this.Player.myStats.Merge(ItemSetTable.getItemSet(Item.Template.ItemSetID).getBonusStatByItemCount(CountItemByItemSet(Item.Template.ItemSetID)));
                            this.Player.Life += ItemSetTable.getItemSet(Item.Template.ItemSetID).getBonusStatByItemCount(CountItemByItemSet(Item.Template.ItemSetID)).GetTotal(EffectEnum.AddVitalite);
                        }
                        Player.Send(new ItemSetAppearMessage(Player, Item.Template.ItemSetID));
                    }
                }
            }
        }

        public int CountItemByItemSet(int itemset)
        {
            lock (ItemsCache)
            {
                return this.ItemsCache.Values.Where(x => x != null && x.Template.ItemSetID == itemset && x.Slot > ItemSlotEnum.SLOT_INVENTAIRE && x.Slot <= ItemSlotEnum.SLOT_BOUCLIER).Count();
            }
        }


        public void MoveItem(int Guid, ItemSlotEnum Slot, bool Packet, int Quantity = 1)
        {
            var Item = this.get(Guid);

            // Item inexistant
            if (Item == null)
            {
                return;
            }

            // Meme endroit ?
            if (Item.Slot == Slot)
            {
                return;
            }

            // Veu equiper un item
            if (Slot > ItemSlotEnum.SLOT_INVENTAIRE && Slot <= ItemSlotEnum.SLOT_BOUCLIER)
            {
                if (Item.Template.Type == 113)
                {
                    if (this.GetItemInSlot(Slot) == null)
                    {
                        this.Player.Send(new TextInformationMessage(TextInformationTypeEnum.ERREUR, 161));
                        return;
                    }
                    if (Item.Quantity > 1)
                    {
                        if (Quantity > Item.Quantity)
                            Quantity = Item.Quantity;
                        int newQ = Item.Quantity - Quantity;
                        if (newQ > 0)
                        {
                            InventoryItemTable.TryCreateItem(Item.TemplateID, this.Player, newQ, -1, Item.GetStats().ToItemStats());
                            UpdateObjectquantity(Item, Quantity);
                        }
                    }
                    var EquipedItemB = this.GetItemInSlot(Slot);

                    var SpeakingTestItem = SpeakingTable.Cache.Values.FirstOrDefault(x => x != null && x.LinkedItem == Item.ID && x.Associated == 0);

                    if (SpeakingTestItem != null)
                    {
                        EquipedItemB.SpeakingID = SpeakingTestItem.ID;
                        SpeakingTestItem.Stats = EquipedItemB.GetStats().ToItemStats();
                        SpeakingTestItem.LinkedItem = EquipedItemB.ID;
                        SpeakingTestItem.Associated = 1;
                        remove(Item.ID);
                        Player.Send(new ObjectRemoveMessage(Item.ID));
                        Player.Send(new ObjectRefreshMessage(EquipedItemB));
                        Player.myMap.SendToMap(new ObjectActualiseMessage(Player));
                        if (Player.Client.GetFight() != null)
                            Player.Client.GetFight().SendToFight(new ObjectActualiseMessage(Player));
                        return;
                    }

                    var ID = DatabaseCache.nextSpeakingId++;
                    String Date = DateTime.Now.Month + "" + DateTime.Now.Day;
                    String InterDate = (DateTime.Now.Month + 3) + "" + DateTime.Now.Day;
                    String Time = DateTime.Now.Hour + "" + DateTime.Now.Minute;

                    Speaking newItem = new Speaking(ID, DateTime.Now.Year, int.Parse(Date), int.Parse(Time), 1, 1, EquipedItemB.Template.Type, EquipedItemB.ID, 0, DateTime.Now.Year, int.Parse(InterDate), int.Parse(Time), DateTime.Now.Year, int.Parse(Date), int.Parse(Time), 1, Item.TemplateID, Item.ID, EquipedItemB.GetStats().ToItemStats());
                    SpeakingTable.New(newItem);
                    EquipedItemB.SpeakingID = ID;
                    remove(Item.ID);
                    Player.Send(new ObjectRemoveMessage(Item.ID));
                    Player.Send(new ObjectRefreshMessage(EquipedItemB));
                    Player.myMap.SendToMap(new ObjectActualiseMessage(Player));
                    if (Player.Client.GetFight() != null)
                        Player.Client.GetFight().SendToFight(new ObjectActualiseMessage(Player));
                    return;
                }

                // Il peu placer l'item dans le slot desiré ?
                if (!ItemTemplateModel.CanPlaceInSlot(ItemTemplateTable.GetTemplate(Item.TemplateID).ItemType, Slot))
                {
                    return;
                }

                // Level requis
                if (ItemTemplateTable.GetTemplate(Item.TemplateID).Level > Player.Level)
                {
                    if (Packet)
                        Player.Send(new ObjectMoveFailMessage(ObjectMoveFailReasonEnum.REASON_LEVEL_REQUIRED));
                    return;
                }

                if (!ItemTemplateTable.GetTemplate(Item.TemplateID).Criterions.Equals("") && !ConditionParser.validConditions(this.Player, ItemTemplateTable.GetTemplate(Item.TemplateID).Criterions))
                {
                    this.Player.Send(new ImMessage("119|43"));
                    return;
                }

                var EquipedItem = this.GetItemInSlot(Slot);

                // Si item deja equipé dans le slot
                if (EquipedItem != null)
                {
                    // Deplacement dans l'inventaire
                    Player.GetStats().UnMerge(EquipedItem.GetStats());
                    this.Player.Life -= EquipedItem.GetStats().GetTotal(EffectEnum.AddVitalite);
                    EquipedItem.Position = -1;
                    Player.Send(new ObjectMoveSucessMessage(EquipedItem.ID, -1));
                }

                // Template deja equipé
                if (this.HasTemplateEquiped(Item.TemplateID))
                {
                    if (Packet)
                        Player.Send(new ObjectMoveFailMessage(ObjectMoveFailReasonEnum.REASON_ALREADY_EQUIPED));
                    return;
                }
                /*var simlarItem = this.getSimilarInventoryItem(Item);
                if (simlarItem != null)
                {
                    this.UpdateObjectquantity(simlarItem, simlarItem.Quantity + Item.Quantity);
                    remove(Item.ID);
                    InventoryItemTable.removeItem(Item.ID);
                    Player.Send(new ObjectRemoveMessage(Item.ID));

                }
                else*/
                if (Item.Quantity > 1)
                {
                    var NewItem = InventoryItemTable.TryCreateItem(Item.TemplateID, this.Player, 1, (short)Slot, Item.Effects);
                    this.UpdateObjectquantity(Item, Item.Quantity - 1);
                }
                else
                {
                    // On modifi la position et fusionne les stats
                    Item.Position = (short)Slot;
                    Player.Send(new ObjectMoveSucessMessage(Item.ID, (short)Slot));
                    Player.Send(new ObjectMoveSucessMessage(Item.ID, (short)Item.Position));
                }

                if (GetItemInSlot(ItemSlotEnum.SLOT_ARME) != null)
                {
                    Player.Send(new ObjectTaskMessage(-1));
                }

                Player.GetStats().Merge(Item.GetStats());
                this.Player.Life += Item.GetStats().GetTotal(EffectEnum.AddVitalite);
                this.Player.Life += Item.GetStats().GetTotal(EffectEnum.AddVie);
                if (Packet)
                {
                    Player.Client.Send(new AccountStatsMessage(Player));
                    Player.myMap.SendToMap(new ObjectActualiseMessage(Player));
                    if (Player.Client.GetFight() != null)
                        Player.Client.GetFight().SendToFight(new ObjectActualiseMessage(Player));
                }
            }
            else
            {
                var NeedActualise = false;

                // Si l'item est equipé, on deduit les stats
                if (Item.Slot > ItemSlotEnum.SLOT_INVENTAIRE && Item.Slot < ItemSlotEnum.SLOT_BOUCLIER)
                {
                    // Retire les stats
                    Player.GetStats().UnMerge(Item.GetStats());
                    this.Player.Life -= Item.GetStats().GetTotal(EffectEnum.AddVitalite);
                    this.Player.Life -= Item.GetStats().GetTotal(EffectEnum.AddVie);
                    if (Player.Life <= 0)
                        Player.Life = 1;
                    if (Packet)
                    {
                        Player.Client.Send(new AccountStatsMessage(this.Player));
                        NeedActualise = true;
                    }
                }

                // On tente de fusionner
                if (!this.TryMergeItem(Item.TemplateID, Item.Effects, Slot, Item.Quantity, Item))
                {
                    Item.Position = (short)Slot;
                    if (Packet)
                        Player.Send(new ObjectMoveSucessMessage(Item.ID, (short)Item.Position));
                }

                if (NeedActualise)
                {
                    Player.myMap.SendToMap(new ObjectActualiseMessage(Player));
                    if (Player.Client.GetFight() != null)
                        Player.Client.GetFight().SendToFight(new ObjectActualiseMessage(Player));
                }
            }
            onMoveItem(Item);
        }

        public void UpdateObjectquantity(InventoryItemModel Item, int Quantity)
        {
            Item.Quantity = Quantity;
            Player.Send(new ObjectQuantityMessage(Item.ID, Item.Quantity));
        }

        public void SerializeAsInventoryContent(StringBuilder SerializedString)
        {
            foreach (var Item in this.ItemsCache)
            {
                SerializedString.Append(Item.Value.ToString());
            }
        }

        public bool hasItemTemplate(int i, int q)
        {
            foreach (InventoryItemModel obj in ItemsCache.Values)
            {
                if ((obj.Position == -1) && (obj.TemplateID == i) && (obj.Quantity >= q))
                {
                    return true;
                }
            }
            return false;
        }

        public bool HasTemplateEquiped(int TemplateId)
        {
            for (int i = 0; i < 16; i++)
            {
                var Item = GetItemInSlot((ItemSlotEnum)i);

                if (Item != null)
                    if (Item.TemplateID == TemplateId)
                        return true;
            }
            return false;
        }


        public Dictionary<long, InventoryItemModel> getCache()
        {
            return this.ItemsCache;
        }

        public String getItemsIDSplitByChar(String splitter)
        {
            StringBuilder str = new StringBuilder();
            if (this.ItemsCache == null || this.ItemsCache.Count == 0) return "";
            foreach (int entry in this.ItemsCache.Keys)
            {
                if (str.Length != 0) str.Append(splitter);
                str.Append(entry);
            }
            return str.ToString();
        }


        public void SerializeAsDisplayEquipment(StringBuilder Packet)
        {
            var Arme = GetItemInSlot(ItemSlotEnum.SLOT_ARME);
            var Coiffe = GetItemInSlot(ItemSlotEnum.SLOT_COIFFE);
            var Cape = GetItemInSlot(ItemSlotEnum.SLOT_CAPE);
            var Familier = GetItemInSlot(ItemSlotEnum.SLOT_FAMILIER);
            var Bouclier = GetItemInSlot(ItemSlotEnum.SLOT_BOUCLIER);

            if (Arme != null)
                Packet.Append(Arme.TemplateID.ToString("x"));
            Packet.Append(',');
            if (Coiffe != null)
            {
                if (Coiffe.SpeakingItem != null)
                {
                    Packet.Append(Coiffe.SpeakingItem.TemplateReal.ToString("x")).Append('~').Append(Coiffe.SpeakingItem.Type).Append('~').Append(Coiffe.SpeakingItem.Masque);
                }
                else
                    Packet.Append(Coiffe.TemplateID.ToString("x"));
            }
            Packet.Append(',');
            if (Cape != null)
            {
                if (Cape.SpeakingItem != null)
                {
                    Packet.Append(Cape.SpeakingItem.TemplateReal.ToString("x")).Append('~').Append(Cape.SpeakingItem.Type).Append('~').Append(Cape.SpeakingItem.Masque);
                }
                else
                    Packet.Append(Cape.TemplateID.ToString("x"));
            }
            Packet.Append(',');
            if (Familier != null)
                Packet.Append(Familier.TemplateID.ToString("x"));
            Packet.Append(',');
            if (Bouclier != null)
                Packet.Append(Bouclier.TemplateID.ToString("x"));
        }

        public String SerializeAsDisplayEquipment()
        {
            var Packet = new StringBuilder();
            var Arme = GetItemInSlot(ItemSlotEnum.SLOT_ARME);
            var Coiffe = GetItemInSlot(ItemSlotEnum.SLOT_COIFFE);
            var Cape = GetItemInSlot(ItemSlotEnum.SLOT_CAPE);
            var Familier = GetItemInSlot(ItemSlotEnum.SLOT_FAMILIER);
            var Bouclier = GetItemInSlot(ItemSlotEnum.SLOT_BOUCLIER);

            if (Arme != null)
                Packet.Append(Arme.TemplateID.ToString("x"));
            Packet.Append(',');
            if (Coiffe != null)
            {
                if (Coiffe.SpeakingItem != null)
                {
                    Packet.Append(Coiffe.SpeakingItem.TemplateReal.ToString("x")).Append('~').Append(Coiffe.SpeakingItem.Type).Append('~').Append(Coiffe.SpeakingItem.Masque);
                }
                else
                    Packet.Append(Coiffe.TemplateID.ToString("x"));
            }
            Packet.Append(',');
            if (Cape != null)
            {
                if (Cape.SpeakingItem != null)
                {
                    Packet.Append(Cape.SpeakingItem.TemplateReal.ToString("x")).Append('~').Append(Cape.SpeakingItem.Type).Append('~').Append(Cape.SpeakingItem.Masque);
                }
                else
                    Packet.Append(Cape.TemplateID.ToString("x"));
            }
            Packet.Append(',');
            if (Familier != null)
                Packet.Append(Familier.TemplateID.ToString("x"));
            Packet.Append(',');
            if (Bouclier != null)
                Packet.Append(Bouclier.TemplateID.ToString("x"));
            return Packet.ToString();
        }

        public void AddKamas(long Value)
        {
            this.Player.Kamas += Value;
        }

        public void SubstractKamas(long Value)
        {
            this.Player.Kamas -= Value;
        }

        public void RemoveItem(InventoryItemModel Item)
        {
            this.ItemsCache.Remove(Item.ID);
        }


        public InventoryItemModel GetItem(long Guid)
        {
            if (!ItemsCache.ContainsKey(Guid))
            {
                return null;
            }
            return ItemsCache[Guid];
        }
    }
}
