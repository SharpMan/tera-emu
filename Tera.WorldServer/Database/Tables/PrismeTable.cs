using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Tera.Libs;
using Tera.WorldServer.Database.Models;

namespace Tera.WorldServer.Database.Tables
{
    public static class PrismeTable
    {
        public static Dictionary<long, Prisme> Cache = new Dictionary<long, Prisme>();
        private static Dictionary<int, StringBuilder> mySerializedPattern = new Dictionary<int, StringBuilder>(){
            { 0, null },
            { 1, null },
            { 2, null },
            { 3, null },
        };

        public static void Load()
        {
            Cache.Clear();
            long time = Environment.TickCount;
            var reader = DatabaseManager.Provider.ExecuteReader("SELECT * FROM prismes");
            while (reader.Read())
            {
                var template = new Prisme()
                {
                    ActorId = reader.GetInt64("id"),
                    Alignement = reader.GetInt32("align"),
                    Level = (byte)reader.GetInt32("grade"),
                    Mapid = reader.GetInt16("mapid"),
                    CellId = reader.GetInt32("cellid"),
                    Honor = reader.GetInt32("honor"),
                    Area = reader.GetInt32("zone"),
                    inFight = -1,
                    Orientation = 1,
                };
                Cache.Add(template.ActorId, template);

                if (template.Map != null)
                    template.Map.SpawnActor(template);
            }
            reader.Close();

            AreaTable.Cache.Values.ToList().ForEach(x => x.onPrismLoaded());
            AreaSubTable.Cache.Values.ToList().ForEach(x => x.onPrismLoaded());

            //Cache.Values.ToList().ForEach(x => x.Intialize());

            Logger.Info("Loaded @'" + Cache.Count + "'@ Prism in @" + (Environment.TickCount - time) + "@ ms");
        }

        public static void Add(Prisme item)
        {
            try
            {
                MySqlCommand Command = new MySqlCommand()
                {
                    Connection = DatabaseManager.Provider.getConnection(),
                    CommandText = "REPLACE INTO `prismes` VALUES(@id,@mapid,@cellid,@align,@grade,@honor,@zone);",
                };
                Command.Prepare();
                Command.Parameters.AddWithValue("@id", item.ActorId);
                Command.Parameters.AddWithValue("@mapid", item.Mapid);
                Command.Parameters.AddWithValue("@cellid", item.CellId);
                Command.Parameters.AddWithValue("@align", item.Alignement);
                Command.Parameters.AddWithValue("@grade", item.Level);
                Command.Parameters.AddWithValue("@honor", item.Honor);
                Command.Parameters.AddWithValue("@zone", item.Area);
                Command.ExecuteNonQuery();
                Cache.Add(item.ActorId, item);
            }
            catch (Exception e)
            {
                Logger.Error("Can't execute query : " + e.ToString());
            }
        }

        public static Prisme getPrism(long id)
        {
            Prisme prism = null;
            Cache.TryGetValue(id, out prism);
            return prism;
        }

        public static void TryDeleteTax(Prisme tc)
        {
            try
            {
                lock (Cache)
                {
                    Cache.Remove(tc.ActorId);
                }

                DatabaseManager.Provider.ExecuteQuery("DELETE FROM prismes WHERE id = '" + tc.ActorId + "'");

            }
            catch (Exception ex)
            {
                Logger.Error("DatabaseEntities::TryDeletePrism : Guid=" + tc.ActorId + " " + ex.ToString());
            }
        }

        public static void DestroyPrismGepositionCache()
        {
            lock (mySerializedPattern) // InvalidOperationException
            {
                foreach (var Key in mySerializedPattern.Keys)
                {
                    mySerializedPattern[Key] = null;
                }
            }
        }

        public static String SerializePrismGeposition(int Align)
        {
            if (mySerializedPattern[Align] == null)
            {
                StringBuilder str = new StringBuilder();
                mySerializedPattern[Align] = new StringBuilder();
                Boolean isFisrt = false;
                int AreaSubCount = 0;
                String geo = "";

                foreach (var subarea in AreaSubTable.Cache.Values)
                {
                    if (!subarea.CanConquest)
                    {
                        continue;
                    }
                    if (isFisrt)
                    {
                        str.Append(";");
                    }
                    str.Append(subarea.ID + "," + (subarea.Alignement == 0 ? -1 : subarea.Alignement) + ",0,");
                    if (getPrism(subarea.Prisme) == null)
                    {
                        str.Append("0,1");
                    }
                    else
                    {
                        str.Append((subarea.Prisme == 0 ? 0 : getPrism(subarea.Prisme).Mapid) + ",1");
                    }
                    isFisrt = true;
                    AreaSubCount++;
                }
                if (Align == 1)
                {
                    str.Append("|" + Area.Bontas);
                }
                else if (Align == 2)
                {
                    str.Append("|" + Area.Brakmars);
                }
                str.Append("|" + AreaTable.Cache.Count + "|");
                isFisrt = false;
                foreach (Area area in AreaTable.Cache.Values)
                {
                    if (area.Alignement == 0)
                    {
                        continue;
                    }
                    if (isFisrt)
                    {
                        str.Append(";");
                    }
                    str.Append(area.ID + "," + area.Alignement + ",1," + (area.Prisme == 0 ? 0 : 1));
                    isFisrt = true;
                }
                
                if (Align == 1)
                {
                    geo += Area.Bontas;
                }
                else if (Align == 2)
                {
                    geo += Area.Brakmars;
                }
                mySerializedPattern[Align].Append(geo).Append("|").Append(AreaSubCount).Append("|").Append((AreaSubCount - (AreaSub.Bontas + AreaSub.Brakmars))).Append("|").Append(str.ToString());
            }

            return mySerializedPattern[Align].ToString();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static int getPrismeID()
        {
            int max = -102;
            foreach (int a in Cache.Keys)
            {
                if (a < max)
                {
                    max = a;
                }
            }
            return max - 3;
        }

    }
}
