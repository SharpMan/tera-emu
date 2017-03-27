using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.WorldServer.Database.Models;

namespace Tera.WorldServer.Database.Tables
{
    public static class MountParkTable
    {
        public static List<MountPark> Cache = new List<MountPark>();

        public static void Load()
        {
            var reader = DatabaseManager.Provider.ExecuteReader("SELECT * FROM mountpark_data");
            while (reader.Read())
            {
                Map map = MapTable.Get(reader.GetInt16("mapid"));
                if (map == null) continue;
                var area = new Models.MountPark(reader.GetInt64("owner"),map,reader.GetInt32("cellid"),reader.GetInt32("size"),reader.GetString("data"),reader.GetInt32("guild"),reader.GetInt32("price"));
                Cache.Add(area);
            }
            reader.Close();

            Logger.Info("Loaded @'" + Cache.Count + "'@ MountPark");
        }

        public static void Update(MountPark model)
        {
            try
            {
                MySqlCommand Command = new MySqlCommand()
                {
                    Connection = DatabaseManager.Provider.getConnection(),
                    CommandText = "REPLACE INTO `mountpark_data` VALUES(@map,@cell,@size,@owner,@guild,@price,@data);",
                };
                Command.Prepare();
                Command.Parameters.AddWithValue("@map", model.get_map().Id);
                Command.Parameters.AddWithValue("@cell", model.get_cellid());
                Command.Parameters.AddWithValue("@size", model.get_size());
                Command.Parameters.AddWithValue("@owner", model.get_owner());
                Command.Parameters.AddWithValue("@guild", (model.get_guild() == null ? -1 : model.get_guild().ID));
                Command.Parameters.AddWithValue("@price", model.get_price());
                Command.Parameters.AddWithValue("@data", (model.parseData()));
                Command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Logger.Error("Can't execute query : " + e.ToString());
            }
        }

        public static Byte CountByGuild(int getId)
        {
            byte i = 0;
            try
            {
                var reader = DatabaseManager.Provider.ExecuteReader("SELECT * FROM mountpark_data WHERE guild='" + getId + "';");
                while (reader.Read())
                {
                    i++;
                }
                reader.Close();
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
            return i;
        }

       
    }
}
