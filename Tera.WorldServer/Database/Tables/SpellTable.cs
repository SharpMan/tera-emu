using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.WorldServer.Database.Models;

namespace Tera.WorldServer.Database.Tables
{
    public static class SpellTable
    {
        public static Dictionary<int, SpellModel> Cache = new Dictionary<int, SpellModel>();

        public static void Load()
        {
            Cache.Clear();
            var reader = DatabaseManager.Provider.ExecuteReader("SELECT * FROM spells");
            while (reader.Read())
            {
                var spell = new SpellModel()
                {
                    ID = reader.GetInt32("id"),
                    Name = reader.GetString("nom"),
                    SpriteID = reader.GetInt32("sprite"),
                    SpriteInfos = reader.GetString("spriteInfos"),
                    Level1 = reader.GetString("lvl1"),
                    Level2 = reader.GetString("lvl2"),
                    Level3 = reader.GetString("lvl3"),
                    Level4 = reader.GetString("lvl4"),
                    Level5 = reader.GetString("lvl5"),
                    Level6 = reader.GetString("lvl6"),
                    EffectTargets = reader.GetString("effectTarget"),
                };
                spell.Initialize();
                Cache.Add(spell.ID, spell);
            }
            reader.Close();

            Logger.Info("Loaded @'" + Cache.Count + "'@ Spells");
        }
    }
}
