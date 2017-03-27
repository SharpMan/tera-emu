using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.WorldServer.Database.Models;

namespace Tera.WorldServer.Database.Tables
{
    public class IObjectTemplateTable
    {
        public static Dictionary<int, IObjectTemplate> Cache = new Dictionary<int, IObjectTemplate>();

        public static void Load()
        {
            Cache.Clear();
            var reader = DatabaseManager.Provider.ExecuteReader("SELECT * FROM interactive_objects_data");
            while (reader.Read())
            {
                var breed = new Models.IObjectTemplate()
                {
                    ID = reader.GetInt32("id"),
                    RespawnTime = reader.GetInt32("respawn"),
                    Duration = reader.GetInt32("duration"),
                    Unk = reader.GetInt32("unknow"),
                    Walakable = reader.GetInt32("walkable") == 1,
                };
                Cache.Add(breed.ID, breed);
            }
            reader.Close();

            Logger.Info("Loaded @'" + Cache.Count + "'@ IO Template");
        }

        public static IObjectTemplate Get(int id)
        {
            if (!Cache.ContainsKey(id))
            {
                return null;
            }
            return Cache[id];
        }

    }
}
