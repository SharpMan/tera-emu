using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.WorldServer.Database.Models;

namespace Tera.WorldServer.Database.Tables
{
    public class SpeakingTable
    {
        public static Dictionary<long, Speaking> Cache = new Dictionary<long, Speaking>();

        public static void Load(List<long> IDS)
        {
            StringBuilder IDSB = new StringBuilder();
            foreach (var items in IDS)
            {
                if (IDSB.Length != 0) IDSB.Append(',');
                IDSB.Append(items);
            }

            var reader = DatabaseManager.Provider.ExecuteReader("SELECT * FROM items_speaking WHERE id IN (" + IDSB.ToString() + ");");

            while (reader.Read())
            {
                var item = new Speaking()
                {
                    ID = reader.GetInt64("id"),
                    LastEatYear = reader.GetInt32("LastEatYear"),
                    LastEatTime = reader.GetInt32("LastEatTime"),
                    LastEatHour = reader.GetInt32("LastEatHour"),
                    Humour = reader.GetInt32("humour"),
                    Masque = reader.GetInt32("masque"),
                    Type = reader.GetInt32("type"),
                    LinkedItem = reader.GetInt64("linkedItem"),
                    EXP = reader.GetInt64("xp"),
                    YearInter = reader.GetInt32("YearInter"),
                    DateInter = reader.GetInt32("DateInter"),
                    HourInter = reader.GetInt32("HourInter"),
                    YearReceive = reader.GetInt32("YearReceived"),
                    DateReceived = reader.GetInt32("DateReceived"),
                    HourReceived = reader.GetInt32("HourReceived"),
                    Associated = reader.GetInt32("Associated"),
                    TemplateReal = reader.GetInt32("TemplateReal"),
                    LivingItem = reader.GetInt64("livingitem"),
                    Stats = reader.GetString("stats"),
                };
                Cache.Add(item.ID, item);
            }

            reader.Close();
            Cache.Values.Where(x => !x.Intialized).ToList().ForEach(x => x.Intialize());
        }

        public static void Add(Speaking item)
        {
            try
            {
                MySqlCommand Command = new MySqlCommand()
                {
                    Connection = DatabaseManager.Provider.getConnection(),
                    CommandText = "REPLACE INTO `items_speaking` VALUES(@guid,@2,@3,@4,@5,@6,@7,@8,@9,@10,@11,@12,@13,@14,@15,@16,@17,@18,@19);",
                };
                Command.Prepare();
                Command.Parameters.AddWithValue("@guid", item.ID);
                Command.Parameters.AddWithValue("@2", item.LastEatYear);
                Command.Parameters.AddWithValue("@3", item.LastEatTime);
                Command.Parameters.AddWithValue("@4", item.LastEatHour);
                Command.Parameters.AddWithValue("@5", item.Humour);
                Command.Parameters.AddWithValue("@6", item.Masque);
                Command.Parameters.AddWithValue("@7", item.Type);
                Command.Parameters.AddWithValue("@8", item.LinkedItem);
                Command.Parameters.AddWithValue("@9", item.EXP);
                Command.Parameters.AddWithValue("@10", item.YearInter);
                Command.Parameters.AddWithValue("@11", item.DateInter);
                Command.Parameters.AddWithValue("@12", item.HourInter);
                Command.Parameters.AddWithValue("@13", item.YearReceive);
                Command.Parameters.AddWithValue("@14", item.DateReceived);
                Command.Parameters.AddWithValue("@15", item.HourReceived);
                Command.Parameters.AddWithValue("@16", item.Associated);
                Command.Parameters.AddWithValue("@17", item.TemplateReal);
                Command.Parameters.AddWithValue("@18", item.LivingItem);
                Command.Parameters.AddWithValue("@19", item.Stats);

                Command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Logger.Error("Can't execute query : " + e.ToString());
            }
        }

        public static void Load(long IDS)
        {
            var reader = DatabaseManager.Provider.ExecuteReader("SELECT * FROM items_speaking WHERE id = " + IDS + ";");

            Speaking item = null;

            while (reader.Read())
            {
                item = new Speaking()
                {
                    ID = reader.GetInt64("id"),
                    LastEatYear = reader.GetInt32("LastEatYear"),
                    LastEatTime = reader.GetInt32("LastEatTime"),
                    LastEatHour = reader.GetInt32("LastEatHour"),
                    Humour = reader.GetInt32("humour"),
                    Masque = reader.GetInt32("masque"),
                    Type = reader.GetInt32("type"),
                    LinkedItem = reader.GetInt64("linkedItem"),
                    EXP = reader.GetInt64("xp"),
                    YearInter = reader.GetInt32("YearInter"),
                    DateInter = reader.GetInt32("DateInter"),
                    HourInter = reader.GetInt32("HourInter"),
                    YearReceive = reader.GetInt32("YearReceived"),
                    DateReceived = reader.GetInt32("DateReceived"),
                    HourReceived = reader.GetInt32("HourReceived"),
                    Associated = reader.GetInt32("Associated"),
                    TemplateReal = reader.GetInt32("TemplateReal"),
                    LivingItem = reader.GetInt64("livingitem"),
                    Stats = reader.GetString("stats"),
                };
                Cache.Add(item.ID, item);
            }

            reader.Close();
            if (item != null) item.Intialize();
        }



        public static void New(Speaking a)
        {
            if (Cache.ContainsKey(a.ID))
            {
                Logger.Error("Duplicate speaking item name " + a.ID);
                return;
            }
            Cache.Add(a.ID, a);
            Add(a);
        }


        public static Speaking GetSpeakingItem(long ID)
        {
            Speaking Item = null;

            Cache.TryGetValue(ID, out Item);

            return Item;
        }

        public static long getNextGuid()
        {
            var reader = DatabaseManager.Provider.ExecuteReader("SELECT MAX(id) AS max FROM items_speaking");
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

    }
}
