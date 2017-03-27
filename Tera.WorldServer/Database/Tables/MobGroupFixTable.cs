using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.Libs.Utils;

namespace Tera.WorldServer.Database.Tables
{
    public static class MobGroupFixTable
    {
        public static void Load()
        {
            int a = 0;
            var reader = DatabaseManager.Provider.ExecuteReader("SELECT * FROM mobgroups_fix");
            while (reader.Read())
            {
                var map = MapTable.Get(reader.GetInt16("mapid"));
                if (map == null)
                {
                    continue;
                }
                var Group = new Couple<int, String>(reader.GetInt32("cellid"), reader.GetString("groupData"));
                map.StaticGroup.Add(Group);
                a++;
            }
            reader.Close();

            Logger.Info("Loaded @'" + a + "'@ MobGroupsFix");
        }

        public static void Update(short Mapid,int Cellid,String Group)
        {
            try
            {
                MySqlCommand Command = new MySqlCommand()
                {
                    Connection = DatabaseManager.Provider.getConnection(),
                    CommandText = "REPLACE INTO `mobgroups_fix` VALUES(@map,@cell,@group);",
                };
                Command.Prepare();
                Command.Parameters.AddWithValue("@map", Mapid);
                Command.Parameters.AddWithValue("@cell", Cellid);
                Command.Parameters.AddWithValue("@group", Group);
                Command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }
}
