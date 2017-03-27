using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.WorldServer.Database.Models;

namespace Tera.WorldServer.Database.Tables
{
    public static class TaxCollectorTable
    {
        public static Dictionary<long, TaxCollector> Cache = new Dictionary<long, TaxCollector>();

        public static void Load()
        {
            Cache.Clear();
            long time = Environment.TickCount;
            var reader = DatabaseManager.Provider.ExecuteReader("SELECT * FROM taxcollector");
            while (reader.Read())
            {
                var template = new TaxCollector()
                {
                   ActorId = reader.GetInt64("guid"),
                   Mapid = reader.GetInt16("mapid"),
                   CellId = reader.GetInt32("cellid"),
                   Orientation = reader.GetInt32("orientation"),
                   GuildID = reader.GetInt32("guild_id"),
                   N1 = reader.GetInt16("N1"),
                   N2 = reader.GetInt16("N2"),
                   ItemList = reader.GetString("objets"),
                   Kamas = reader.GetInt64("kamas"),
                   XP = reader.GetInt64("xp"),
                };
                Cache.Add(template.ActorId, template);

                if (template.Map != null)
                    template.Map.SpawnActor(template);
            }
            reader.Close();

            Cache.Values.ToList().ForEach(x => x.Intialize());

            Logger.Info("Loaded @'" + Cache.Count + "'@ TaxCollector in @" + (Environment.TickCount - time) + "@ ms");
        }


        public static void TryDeleteTax(TaxCollector tc)
        {
            try
            {
                lock (Cache)
                {
                    Cache.Remove(tc.ActorId);
                    tc.Items.Values.ToList().ForEach(x => InventoryItemTable.removeItem(x.ID));
                }

                DatabaseManager.Provider.ExecuteQuery("DELETE FROM taxcollector WHERE guid = '" + tc.ActorId + "'");

            }
            catch (Exception ex)
            {
                Logger.Error("DatabaseEntities::TryDeleteperco : Guid=" + tc.ActorId + " " + ex.ToString());
            }
        }


        public static void Add(TaxCollector item)
        {
            try
            {
                MySqlCommand Command = new MySqlCommand()
                {
                    Connection = DatabaseManager.Provider.getConnection(),
                    CommandText = "REPLACE INTO `taxcollector` VALUES(@guid,@map,@cell,@or,@guild,@n1,@n2,@item,@kamas,@xp);",
                };
                Command.Prepare();
                Command.Parameters.AddWithValue("@guid", item.ActorId);
                Command.Parameters.AddWithValue("@map", item.Mapid);
                Command.Parameters.AddWithValue("@cell", item.CellId);
                Command.Parameters.AddWithValue("@or", item.Orientation);
                Command.Parameters.AddWithValue("@n1", item.N1);
                Command.Parameters.AddWithValue("@guild", item.GuildID);
                Command.Parameters.AddWithValue("@n2", item.N2);
                Command.Parameters.AddWithValue("@item", item.getItemsId());
                Command.Parameters.AddWithValue("@kamas", item.Kamas);
                Command.Parameters.AddWithValue("@xp", item.XP);

                Command.ExecuteNonQuery();
                Cache.Add(item.ActorId, item);
            }
            catch (Exception e)
            {
                Logger.Error("Can't execute query : " + e.ToString());
            }
        }

        public static void Update(TaxCollector item)
        {
            try
            {
                MySqlCommand Command = new MySqlCommand()
                {
                    Connection = DatabaseManager.Provider.getConnection(),
                    CommandText = "UPDATE `taxcollector` set objets = @objets, kamas = @kamas, xp = @xp WHERE guid= @guid;",
                };
                Command.Prepare();
                Command.Parameters.AddWithValue("@objets", item.getItemsId());
                Command.Parameters.AddWithValue("@kamas", item.Kamas);
                Command.Parameters.AddWithValue("@xp", item.XP);
                Command.Parameters.AddWithValue("@guid", item.ActorId);

                Command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }


        public static TaxCollector GetPerco(long perco)
        {
            if (!Cache.ContainsKey(perco))
            {
                return null;
            }
            return Cache[perco];
        }

        /*public static void RemovePerco(TaxCollector Perco)
        {
            Perco.Items.Values.ToList().ForEach(x => InventoryItemTable.removeItem(x.ID));
            Cache.Remove(Perco.ActorId);
        }*/

        public static long getNextGuid()
        {
            int i = -50;
            var reader = DatabaseManager.Provider.ExecuteReader("SELECT `guid` FROM `taxcollector` ORDER BY `guid` ASC LIMIT 0 , 1;");
            while (reader.Read())
            {
                try
                {
                    i = reader.GetInt32("guid") - 1;
                }
                catch (Exception e) { }
            }
            reader.Close();
            return i;
        }



    }
}
