using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.WorldServer.Database.Models;

namespace Tera.WorldServer.Database.Tables
{
    public class AreaSubTable
    {
        public static Dictionary<int, AreaSub> Cache = new Dictionary<int, AreaSub>();


        public static void Save(AreaSub item)
        {
            try
            {
                MySqlCommand Command = new MySqlCommand()
                {
                    Connection = DatabaseManager.Provider.getConnection(),
                    CommandText = "REPLACE INTO `subarea_data` VALUES(@id,@area,@alignement,@name,@conquest,@prisme);",
                };
                Command.Prepare();
                Command.Parameters.AddWithValue("@id", item.ID);
                Command.Parameters.AddWithValue("@area", item.areaID);
                Command.Parameters.AddWithValue("@alignement", item.Alignement);
                Command.Parameters.AddWithValue("@name", item.Name);
                Command.Parameters.AddWithValue("@conquest", item.CanConquest ? 0 : 1);
                Command.Parameters.AddWithValue("@prisme", item.Prisme);
                Command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Logger.Error("Can't execute query : " + e.ToString());
            }
        }


        public static void Load()
        {
            Cache.Clear();
            var reader = DatabaseManager.Provider.ExecuteReader("SELECT * FROM subarea_data");
            while (reader.Read())
            {
                var area = new Models.AreaSub()
                {
                    areaID = reader.GetInt32("area"),
                    ID = reader.GetInt32("id"),
                    Name = reader.GetString("name"),
                    Alignement = reader.GetInt32("alignement"),
                    CanConquest = (reader.GetInt32("conquest") == 0),
                    Prisme = reader.GetInt64("prisme"),
                };
                area.Intialize();
                Cache.Add(area.ID, area);
            }
            reader.Close();

            Logger.Info("Loaded @'" + Cache.Count + "'@ SubArea");
        }

        public static float getWorldBalance(int align)
        {
            int cant = 0;
            foreach (AreaSub subarea in Cache.Values)
            {
                if (subarea.Alignement == align)
                {
                    cant++;
                }
            }
            if (cant == 0)
            {
                return 0.0F;
            }
            return (float)Math.Round((double)10 * cant / 4 / 10);
        }

        public static AreaSub Get(int id)
        {
            if (!Cache.ContainsKey(id))
            {
                return null;
            }
            return Cache[id];
        }
    }
}
