using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.WorldServer.Database.Models;

namespace Tera.WorldServer.Database.Tables
{
    public class AreaTable
    {
        public static Dictionary<int, AreaSuper> SuperAreas = new Dictionary<int, AreaSuper>();
        public static Dictionary<int, Area> Cache = new Dictionary<int, Area>();


        public static void Load()
        {
            Cache.Clear();
            var reader = DatabaseManager.Provider.ExecuteReader("SELECT * FROM area_data");
            while (reader.Read())
            {
                var area = new Models.Area()
                {
                    val = reader.GetInt32("superarea"),
                    ID = reader.GetInt32("id"),
                    Name = reader.GetString("name"),
                    Alignement = reader.GetInt32("align"),
                    Prisme = reader.GetInt64("prisme"),
                };
                area.Intialize();
                Cache.Add(area.ID, area);
            }
            reader.Close();

            Logger.Info("Loaded @'" + Cache.Count + "'@ Area");
        }

        public static void Update(Area item)
        {
            try
            {
                MySqlCommand Command = new MySqlCommand()
                {
                    Connection = DatabaseManager.Provider.getConnection(),
                    CommandText = "UPDATE `area_data` set align = @align, prisme = @prisme WHERE id= @id;",
                };
                Command.Prepare();
                Command.Parameters.AddWithValue("@align", item.Alignement);
                Command.Parameters.AddWithValue("@prisme", item.Prisme);
                Command.Parameters.AddWithValue("@id", item.ID);

                Command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        public static Area Get(int id)
        {
            if (!Cache.ContainsKey(id))
            {
                return null;
            }
            return Cache[id];
        }

        public static float getAreaBalance(Area area, int align)
        {
            int cant = 0;
            foreach (AreaSub subarea in AreaSubTable.Cache.Values)
            {
                if ((subarea.area == area) && (subarea.Alignement == align))
                {
                    cant++;
                }
            }
            if (cant == 0)
            {
                return 0.0F;
            }
            return (float)Math.Round((double)1000 * cant / area.subAreas.Count / 10);
        }


        public static AreaSuper GetSuperArea(int id)
        {
            if (!SuperAreas.ContainsKey(id))
            {
                return null;
            }
            return SuperAreas[id];
        }

        public static void Add(AreaSuper AS)
        {
            SuperAreas.Add(AS.ID, AS);
        }

    }
}
