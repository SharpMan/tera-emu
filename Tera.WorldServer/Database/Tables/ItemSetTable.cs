using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.WorldServer.Database.Models;

namespace Tera.WorldServer.Database.Tables
{
    public static class ItemSetTable
    {
        private static Dictionary<int, ItemSetModel> Cache = new Dictionary<int, ItemSetModel>();

        public static void Load()
        {
            Cache.Clear();
            long time = Environment.TickCount;
            var reader = DatabaseManager.Provider.ExecuteReader("SELECT * FROM itemsets");
            while (reader.Read())
            {
                var area = new ItemSetModel()
                {
                    ID = reader.GetInt32("ID"),
                    StringItems = reader.GetString("items"),
                    StringBonus = reader.GetString("bonus")
                };
                area.Initialize();
                Cache.Add(area.ID, area);
            }
            reader.Close();

            Logger.Info("Loaded @'" + Cache.Count + "'@ ItemSet in @" + (Environment.TickCount - time) + "@ ms");
        }

        public static ItemSetModel getItemSet(int ID)
        {
            if (!Cache.ContainsKey(ID))
                return null;
            return Cache[ID];
        }
    }
}
