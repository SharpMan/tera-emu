using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.World.Character;

namespace Tera.WorldServer.Database.Tables
{
    public static class InventoryItemTable
    {
        public static Dictionary<long, InventoryItemModel> Items = new Dictionary<long, InventoryItemModel>();

        public static void Add(InventoryItemModel item)
        {
            try
            {
                MySqlCommand Command = new MySqlCommand()
                {
                    Connection = DatabaseManager.Provider.getConnection(),
                    CommandText = "REPLACE INTO `inventory_item` VALUES(@guid,@template,@qua,@pos,@stats,@speaking);",
                };
                Command.Prepare();
                Command.Parameters.AddWithValue("@guid", item.ID);
                Command.Parameters.AddWithValue("@template", item.TemplateID);
                Command.Parameters.AddWithValue("@qua", item.Quantity);
                Command.Parameters.AddWithValue("@pos", item.Position);
                Command.Parameters.AddWithValue("@stats", item.Effects);
                Command.Parameters.AddWithValue("@speaking", item.SpeakingID);
                Command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Logger.Error("Can't execute query : " + e.ToString());
            }
        }

        public static InventoryItemModel Load(long id)
        {
            var reader = DatabaseManager.Provider.ExecuteReader("SELECT * FROM inventory_item WHERE guid = "+id);
            InventoryItemModel itemToReturn = null;
            while (reader.Read())
            {
                itemToReturn = new InventoryItemModel()
                {
                    ID = reader.GetInt64("guid"),
                    TemplateID = reader.GetInt32("template"),
                    Quantity = reader.GetInt32("qua"),
                    Position = reader.GetInt32("pos"),
                    Effects = reader.GetString("stats"),
                    SpeakingID = reader.GetInt64("speaking"),
                };
                addItem(itemToReturn, false);
            }
            reader.Close();
            if (itemToReturn.SpeakingID != 0)
                SpeakingTable.Load(itemToReturn.ID);
            return itemToReturn;
        }

        public static void Update(InventoryItemModel item)
        {
            try
            {
                MySqlCommand Command = new MySqlCommand()
                {
                    Connection = DatabaseManager.Provider.getConnection(),
                    CommandText = "UPDATE `inventory_item` set template = @template, qua = @qua, pos = @pos, stats = @stats , speaking = @speaking WHERE guid= @guid;",
                };
                Command.Prepare();
                Command.Parameters.AddWithValue("@guid", item.ID);
                Command.Parameters.AddWithValue("@template", item.TemplateID);
                Command.Parameters.AddWithValue("@qua", item.Quantity);
                Command.Parameters.AddWithValue("@pos", item.Position);
                Command.Parameters.AddWithValue("@stats", item.GetStats().ToItemStats(true));
                Command.Parameters.AddWithValue("@speaking", item.SpeakingID);
                Command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        public static void Update(CharacterInventory cache)
        {
            try
            {
                MySqlCommand Command;
                foreach (String id in cache.getItemsIDSplitByChar(":").Split(':'))
                {
                    int guid;
                    try
                    {
                        guid = int.Parse(id);
                    }
                    catch (Exception e) { continue; }
                    InventoryItemModel item = InventoryItemTable.getItem(guid);
                    if (item == null)
                    {
                        continue;
                    }
                    Command = new MySqlCommand()
                    {
                        Connection = DatabaseManager.Provider.getConnection(),
                        CommandText = "UPDATE `inventory_item` set template = @template, qua = @qua, pos = @pos, stats = @stats , speaking = @speaking WHERE guid= @guid;",
                    };
                    Command.Prepare();
                    Command.Parameters.AddWithValue("@guid", item.ID);
                    Command.Parameters.AddWithValue("@template", item.TemplateID);
                    Command.Parameters.AddWithValue("@qua", item.Quantity);
                    Command.Parameters.AddWithValue("@pos", item.Position);
                    Command.Parameters.AddWithValue("@stats", item.GetStats().ToItemStats(true));
                    Command.Parameters.AddWithValue("@speaking", item.SpeakingID);
                    Command.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Logger.Error("Can't execute query : " + e.ToString());
            }
        }

        public static void Delete(long id)
        {
            try
            {
                DatabaseManager.Provider.ExecuteQuery("DELETE FROM inventory_item WHERE guid = '" + id + "'");
            }
            catch (Exception e)
            {
                Logger.Error("Can't execute query : " + e.ToString());
            }
        }

        public static long getNextGuid()
        {
            var reader = DatabaseManager.Provider.ExecuteReader("SELECT MAX(guid) AS max FROM inventory_item");
            long id = 0;
            if (reader.Read())
            {
                try
                {
                    id = reader.GetInt64("max");
                }
                catch (Exception e) { }
            }
            reader.Close();
            return id;
        }


        public static void addItem(InventoryItemModel item, bool saveSQL)
        {
            if (Items.ContainsKey(item.ID))
            {
                Items.Remove(item.ID);
            }
            Items.Add(item.ID, item);
            if (saveSQL)
            {
                Add(item);
            }
        }

        public static InventoryItemModel getItem(long guid)
        {
            if (!Items.ContainsKey(guid))
            {
                return null;
            }
            return Items[guid];
        }

        public static void removeItem(long guid)
        {
            if (Items.ContainsKey(guid))
            {
                Items.Remove(guid);
                Delete(guid);
            }
        }

        public static void Load(string ids)
        {
            var reader = DatabaseManager.Provider.ExecuteReader("SELECT * FROM inventory_item WHERE guid IN (" + ids + ");");
            List<long> LivingItem = new List<long>();

            while (reader.Read())
            {
                var item = new InventoryItemModel()
                {
                    ID = reader.GetInt64("guid"),
                    TemplateID = reader.GetInt32("template"),
                    Quantity = reader.GetInt32("qua"),
                    Position = reader.GetInt32("pos"),
                    Effects = reader.GetString("stats"),
                    SpeakingID = reader.GetInt64("speaking"),
                };
                if (item.SpeakingID != 0)
                    LivingItem.Add(item.SpeakingID);
                addItem(item, false);
            }

            reader.Close();

            if (LivingItem.Count > 0)
                SpeakingTable.Load(LivingItem);
        }



        public static InventoryItemModel TryCreateItem(int templateId, int quantity = 1, short position = -1, string Stats = null, Boolean useMax = false)
        {
            if (!ItemTemplateTable.Cache.ContainsKey(templateId)) // Template inexistant
                return null;

            // Recup template
            var Template = ItemTemplateTable.GetTemplate(templateId);

            // Creation

            var Item = new InventoryItemModel()
            {
                ID = DatabaseCache.nextItemGuid++,
                TemplateID = templateId,
                Position = position,
                Quantity = quantity,
                Effects = (Stats == null ? Template.GenerateStats().ToItemStats() : Stats)
            };

            Item.GetStats();
            addItem(Item, true);

            return Item;
        }

        public static InventoryItemModel TryCreateItem(int templateId, Player Character, int quantity = 1, short position = -1, string Stats = null,Boolean useMax = false)
        {
            if (!ItemTemplateTable.Cache.ContainsKey(templateId)) // Template inexistant
                return null;

            // Recup template
            var Template = ItemTemplateTable.GetTemplate(templateId);

            // Creation

            var Item = new InventoryItemModel()
                {
                    ID = DatabaseCache.nextItemGuid++,
                    TemplateID = templateId,
                    Position = position,
                    Quantity = quantity,
                    Effects = (Stats == null ? (Template.Type == 113 ? Template.StatsTemplate : Template.GenerateStats(useMax).ToItemStats()) : Stats)
                };

            Item.GetStats();

            // Ajout de l'item dans l'inventaire
            if (Character.InventoryCache.Add(Item))
            {
                addItem(Item, true);
            }
            return Item;
        }

        public static InventoryItemModel TryCreateItem(int templateId, Mount Mount, int quantity = 1, short position = -1, string Stats = null, Boolean useMax = false)
        {
            if (!ItemTemplateTable.Cache.ContainsKey(templateId)) // Template inexistant
                return null;

            // Recup template
            var Template = ItemTemplateTable.GetTemplate(templateId);

            // Creation

            var Item = new InventoryItemModel()
            {
                ID = DatabaseCache.nextItemGuid++,
                TemplateID = templateId,
                Position = position,
                Quantity = quantity,
                Effects = (Stats == null ? Template.GenerateStats().ToItemStats() : Stats)
            };

            Item.GetStats();

            // Ajout de l'item dans l'inventaire
            Mount.Items.Add(Item);
            addItem(Item, true);
            
            return Item;
        }

        public static InventoryItemModel TryCreateItem(int templateId, TaxCollector TCollector, int quantity = 1, short position = -1, string Stats = null, Boolean useMax = false)
        {
            if (!ItemTemplateTable.Cache.ContainsKey(templateId)) // Template inexistant
                return null;

            // Recup template
            var Template = ItemTemplateTable.GetTemplate(templateId);

            // Creation

            var Item = new InventoryItemModel()
            {
                ID = DatabaseCache.nextItemGuid++,
                TemplateID = templateId,
                Position = position,
                Quantity = quantity,
                Effects = (Stats == null ? Template.GenerateStats().ToItemStats() : Stats)
            };

            Item.GetStats();

            // Ajout de l'item dans l'inventaire
            TCollector.Items.Add(Item.ID,Item);
            addItem(Item, true);

            return Item;
        }


    }
}
