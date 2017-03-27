using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.WorldServer.Database.Models;

namespace Tera.WorldServer.Database.Tables
{
    public static class ZaapTable
    {
        public static Dictionary<int, int> Cache = new Dictionary<int, int>();

        public static void Load()
        {
            Cache.Clear();
            var reader = DatabaseManager.Provider.ExecuteReader("SELECT * FROM zaaps");
            while (reader.Read())
            {
                Cache.Add(reader.GetInt32("mapID"), reader.GetInt32("cellID"));
            }
            reader.Close();

            Logger.Info("Loaded @'" + Cache.Count + "'@ Zaaps");
        }

        public static int GetCell(int mapid)
        {
            if (Cache.ContainsKey(mapid))
            {
                return Cache[mapid];
            }
            return -1;
        }

        public static int calculZaapCost(Map map1, Map map2)
        {
            return 10 * (Math.Abs(map2.X - map1.X) + Math.Abs(map2.Y - map1.Y) - 1);
        }

    }
}
