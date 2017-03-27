using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.WorldServer.Database.Models;

namespace Tera.WorldServer.Database.Tables
{
    public class MonsterTable
    {
        public static Dictionary<int, Monster> Cache = new Dictionary<int, Monster>();

        public static void Load()
        {
            Cache.Clear();
            var reader = DatabaseManager.Provider.ExecuteReader("SELECT * FROM monsters");
            while (reader.Read())
            {
                var mob = new Monster()
                {
                    ID = reader.GetInt32("id"),
                    Name = reader.GetString("name"),
                    Look = reader.GetInt32("gfxID"),
                    Alignement = reader.GetInt32("align"),
                    Grades = reader.GetString("grades"),
                    Colors = reader.GetString("colors"),
                    Stats = reader.GetString("stats"),
                    Spells = reader.GetString("spells"),
                    Life = reader.GetString("pdvs"),
                    Points = reader.GetString("points"),
                    Inits = reader.GetString("inits"),
                    MinKamas = reader.GetInt32("minKamas"),
                    MaxKamas = reader.GetInt32("maxKamas"),
                    Experiences = reader.GetString("exps"),
                    AI_TYPE = reader.GetInt32("AI_Type"),
                    Capturable = reader.GetInt32("capturable"),
                    staticFighterEffects = reader.GetString("staticFighterEffects"),
                };
                Cache.Add(mob.ID, mob);
            }
            reader.Close();

            Logger.Info("Loaded @'" + Cache.Count + "'@ Monsters");

        }

        public static Monster GetMonster(int guid)
        {
            if (!Cache.ContainsKey(guid))
            {
                return null;
            }
            return Cache[guid];
        }
    }
}
