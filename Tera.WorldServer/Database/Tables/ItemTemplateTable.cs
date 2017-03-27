using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.World.Actions;

namespace Tera.WorldServer.Database.Tables
{
    public static class ItemTemplateTable
    {
        public static Dictionary<int, Models.ItemTemplateModel> Cache = new Dictionary<int, Models.ItemTemplateModel>();

        public static void Load()
        {
            Cache.Clear();
            long time = Environment.TickCount;
            var reader = DatabaseManager.Provider.ExecuteReader("SELECT * FROM item_template");
            while (reader.Read())
            {
                var template = new Models.ItemTemplateModel()
                {
                    ID = reader.GetInt32("id"),
                    Type = reader.GetInt32("type"),
                    Name = reader.GetString("name"),
                    Level = reader.GetInt32("level"),
                    StatsTemplate = reader.GetString("statsTemplate"),
                    Pods = reader.GetInt32("pod"),
                    ItemSetID = reader.GetInt32("panoplie"),
                    Price = reader.GetInt32("prix"),
                    Criterions = reader.GetString("condition"),
                    WeaponInfos = reader.GetString("armesInfos"),
                    AvgPrice = reader.GetInt64("avgPrice"),
                    Sold = reader.GetInt64("sold")
                };
                template.Initialize();
                Cache.Add(template.ID, template);
            }
            reader.Close();

            Logger.Info("Loaded @'" + Cache.Count + "'@ item templates in @" + (Environment.TickCount - time) + "@ ms");
        }

        public static void UpdateAVGPrice()
        {
            try
            {
                MySqlCommand Command;
                foreach (var BHI in Cache.Values.Where(x => x.Sold != 0))
                {
                    Command = new MySqlCommand()
                    {
                        Connection = DatabaseManager.Provider.getConnection(),
                        CommandText = "UPDATE item_template SET sold = @sold, avgPrice = @avp WHERE id = @id;",
                    };
                    Command.Prepare();
                    Command.Parameters.AddWithValue("@sold", BHI.Sold);
                    Command.Parameters.AddWithValue("@avp", BHI.AvgPrice);
                    Command.Parameters.AddWithValue("@id", BHI.ID);
                    Command.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Logger.Error("Can't execute query : " + e.ToString());
            }
        }

        public static void LoadItemActions()
        {
            int nb = 0;
            var reader = DatabaseManager.Provider.ExecuteReader("SELECT * FROM use_item_actions");
            while (reader.Read())
            {
                int id = reader.GetInt32("template");
                int type = reader.GetInt32("type");
                String args = reader.GetString("args");
                if (GetTemplate(id) == null)
                {
                    continue;
                }
                GetTemplate(id).addAction(new ActionModel(type, args, ""));
                nb++;
            }
            reader.Close();

            Logger.Info("Loaded @'" + nb + "'@ Item Actions");
        }

        public static ItemTemplateModel GetTemplate(int id)
        {
            lock (Database.Tables.ItemTemplateTable.Cache)
            {
                if (Database.Tables.ItemTemplateTable.Cache.ContainsKey(id))
                {
                    return Database.Tables.ItemTemplateTable.Cache[id];
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
