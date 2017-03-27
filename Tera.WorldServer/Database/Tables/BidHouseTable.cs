using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Tera.Libs;
using Tera.WorldServer.Database.Models;

namespace Tera.WorldServer.Database.Tables
{
    public static class BidHouseTable
    {
        public static Dictionary<short, BidHouse> Cache = new Dictionary<short, BidHouse>();
        public static Dictionary<int, Dictionary<int, List<BidHouseItem>>> BHITEMS = new Dictionary<int, Dictionary<int, List<BidHouseItem>>>();

        public static void Load()
        {
            Cache.Clear();
            var reader = DatabaseManager.Provider.ExecuteReader("SELECT * FROM bidhouse");
            while (reader.Read())
            {
                var area = new Models.BidHouse()
                {
                    MapID = reader.GetInt16("map"),
                    CategoriesString = reader.GetString("categories"),
                    SellTaxe = reader.GetFloat("sellTaxe"),
                    levelMax = reader.GetInt16("lvlMax"),
                    countItem = reader.GetInt16("countItem"),
                    sellTime = reader.GetInt16("sellTime"),
                };
                area.Initialize();
                Cache.Add(area.MapID, area);
            }
            reader.Close();

            Logger.Info("Loaded @'" + Cache.Count + "'@ BidHouse");
            LineID = getNextGuid();
        }


        public static void LoadItems()
        {
            var reader = DatabaseManager.Provider.ExecuteReader("SELECT * FROM bidhouse_items");
            int nbr = 0;
            var BItems = new List<BidHouseItem>();
            while (reader.Read())
            {
                //TODO Load item after consulting HDV .. but i think not good idea
                BidHouse bHouse = null;
                var ItemToDelete = new List<long>();
                if (!Cache.TryGetValue(reader.GetInt16("map"), out bHouse))
                {
                    continue;
                }
                
                var BItem = new BidHouseItem()
                {
                    Price = reader.GetInt64("price"),
                    Quantity = reader.GetInt32("count"),
                    Owner = reader.GetInt32("owner"),
                    ItemID = reader.GetInt64("item"),
                    BH = bHouse
                };

                BItems.Add(BItem);
               
                nbr++;
            }
            reader.Close();
            BItems.ForEach(x => x.Initialize());

            Logger.Info("Loaded @'" + nbr + "'@ BidHouse Items");
        }

        public static void Update(List<BidHouseItem> list)
        {
            try
            {
                DatabaseManager.Provider.ExecuteQuery("TRUNCATE TABLE `bidhouse_items`");
            }
            catch (Exception e)
            {
                Logger.Error("Can't execute query : " + e.ToString());
            }
            try
            {
                MySqlCommand Command;
                foreach (var BHI in list)
                {
                    if (BHI.Owner == -1)
                        continue;
                    Command = new MySqlCommand()
                    {
                        Connection = DatabaseManager.Provider.getConnection(),
                        CommandText = "INSERT INTO `bidhouse_items` VALUES(@map,@owner,@price,@count,@item);",
                    };
                    Command.Prepare();
                    Command.Parameters.AddWithValue("@map", BHI.MapID);
                    Command.Parameters.AddWithValue("@owner", BHI.Owner);
                    Command.Parameters.AddWithValue("@price", BHI.Price);
                    Command.Parameters.AddWithValue("@count", BHI.getQuantity(false));
                    Command.Parameters.AddWithValue("@item", BHI.Item.ID);
                    Command.ExecuteNonQuery();
                }
                ItemTemplateTable.UpdateAVGPrice();
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
                DatabaseManager.Provider.ExecuteQuery("DELETE FROM bidhouse_items WHERE item = '" + id + "'");
            }
            catch (Exception e)
            {
                Logger.Error("Can't execute query : " + e.ToString());
            }
        }

        public static void Add(BidHouseItem BHI)
        {
            try
            {
                MySqlCommand Command = new MySqlCommand()
                {
                    Connection = DatabaseManager.Provider.getConnection(),
                    CommandText = "REPLACE INTO `bidhouse_items` VALUES(@map,@owner,@price,@count,@item);",
                };
                Command.Prepare();
                Command.Parameters.AddWithValue("@map",  BHI.MapID);
                Command.Parameters.AddWithValue("@owner", BHI.Owner);
                Command.Parameters.AddWithValue("@price", BHI.Price);
                Command.Parameters.AddWithValue("@count", BHI.getQuantity(false));
                Command.Parameters.AddWithValue("@item", BHI.Item.ID);
                Command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Logger.Error("Can't execute query : " + e.ToString());
            }
        }

        public static int getNextGuid()
        {
            var reader = DatabaseManager.Provider.ExecuteReader("SELECT MAX(id) AS max FROM bidhouse");
            int id = 0;
            if (reader.Read())
            {
                try
                {
                    id = reader.GetInt32("max");
                }
                catch (Exception e) { }
            }
            reader.Close();
            return id;
        }


        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void addBidHouseItem(int owner, int hdvID, BidHouseItem BHI)
        {
            if (!BHITEMS.ContainsKey(owner))
                BHITEMS.Add(owner, new Dictionary<int, List<BidHouseItem>>());
            if (!BHITEMS[owner].ContainsKey(hdvID))
                BHITEMS[owner].Add(hdvID, new List<BidHouseItem>());
            BHITEMS[owner][hdvID].Add(BHI);
        }


        private static int LineID = 0;

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static int NextLineID()
        {
            LineID += 1;
            return LineID;
        }

    }
}
