using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.WorldServer.Database.Models;

namespace Tera.WorldServer.Database.Tables
{
    public class NpcTemplateTable
    {
        public static Dictionary<long, NpcTemplateModel> Cache = new Dictionary<long, NpcTemplateModel>();

        public static void Load()
        {
            Cache.Clear();
            var reader = DatabaseManager.Provider.ExecuteReader("SELECT * FROM npc_template");
            while (reader.Read())
            {
                var npc = new NpcTemplateModel()
                {
                    ID = reader.GetInt64("id"),
                    BonusValue = reader.GetInt32("bonusValue"),
                    SkinID = reader.GetInt32("gfxID"),
                    ScaleX = reader.GetInt32("scaleX"),
                    ScaleY = reader.GetInt32("scaleY"),
                    Sexe = reader.GetInt32("sex"),
                    Color1 = reader.GetInt32("color1"),
                    Color2 = reader.GetInt32("color2"),
                    Color3 = reader.GetInt32("color3"),
                    Accessories = reader.GetString("accessories"),
                    ExtraClip = reader.GetInt32("extraClip"),
                    CustomArtWork = reader.GetInt32("customArtWork"),
                    InitQuestion = reader.GetInt32("initQuestion"),
                    Ventes = reader.GetString("ventes"),
                };
                Cache.Add(npc.ID, npc);
            }
            reader.Close();

            Logger.Info("Loaded @'" + Cache.Count + "'@ NPC Templates");
        }

        public static void LoadPlaces()
        {
            var reader = DatabaseManager.Provider.ExecuteReader("SELECT * FROM npcs");
            int nbr = 0;
            while (reader.Read())
            {
                Map map = MapTable.Cache.FirstOrDefault(x => x.Key == reader.GetInt32("mapid")).Value;
                if (map != null)
                {
                    map.addNpc(reader.GetInt32("npcid"), reader.GetInt32("cellid"), reader.GetInt32("orientation"));
                    nbr++;
                }
            }
            reader.Close();

            Logger.Info("Loaded @'" + nbr + "'@ NPC Places");
        }

        public static void Add(int npc,Player character)
        {
            try
            {
                MySqlCommand Command = new MySqlCommand()
                {
                    Connection = DatabaseManager.Provider.getConnection(),
                    CommandText = "REPLACE INTO `npcs` VALUES(@mapid,@npcid,@cellid,@orientation);",
                };
                Command.Prepare();
                Command.Parameters.AddWithValue("@npcid", npc);
                Command.Parameters.AddWithValue("@cellid", character.CellId);
                Command.Parameters.AddWithValue("@mapid", character.Map);
                Command.Parameters.AddWithValue("@orientation", character.Orientation);
                Command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Logger.Error("Can't execute query : " + e.ToString());
            }
        }
    }
}
