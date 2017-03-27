using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.WorldServer.Utils;

namespace Tera.WorldServer.Database.Tables
{
    public class ExpFloorTable
    {
        public static Dictionary<int, Models.ExpFloorModel> Cache = new Dictionary<int, Models.ExpFloorModel>();

        public static void Load()
        {
            Cache.Clear();
            var reader = DatabaseManager.Provider.ExecuteReader("SELECT * FROM exp_data");
            while (reader.Read())
            {
                var floor = new Models.ExpFloorModel()
                {
                    ID = reader.GetInt32("Level"),
                    Character = reader.GetInt64("Character"),
                    Job = reader.GetInt32("Job"),
                    Mount = reader.GetInt32("Mount"),
                    PvP = reader.GetInt32("Pvp"),
                    Living = reader.GetInt32("Living"),
                    Guild = reader.GetInt64("Character") * 10L,
                };
                Cache.Add(floor.ID, floor);
            }
            reader.Close();

            Logger.Info("Loaded @'" + Cache.Count + "'@ exp floors");
        }

        public static Database.Models.ExpFloorModel GetFloorByLevel(int level)
        {
            if (level > Cache.Keys.Last())
                level = Cache.Keys.Last();
            if (ExpFloorTable.Cache.ContainsKey(level))
            {
                return ExpFloorTable.Cache[level];
            }
            else
            {
                return null;
            }
        }

        public static Database.Models.ExpFloorModel GetFloorByCharacterExp(int exp)
        {
            Database.Models.ExpFloorModel floor = null;
            foreach (var f in ExpFloorTable.Cache.Values)
            {
                if (f.Character <= exp)
                {
                    floor = f;
                }
            }
            return floor;

        }

        public static int getObviXpMax(int _lvl)
        {
            if (_lvl >= 20) _lvl = 19;
            if (_lvl <= 1) _lvl = 1;
            return (int)ExpFloorTable.Cache[_lvl + 1].Living;
        }

        public static long getGuildXpMax(int _lvl)
        {
            if (_lvl >= Settings.AppSettings.GetIntElement("World.GuildMaxLevel"))
            {
                _lvl = Settings.AppSettings.GetIntElement("World.GuildMaxLevel") - 1;
            }
            if (_lvl <= 1)
            {
                _lvl = 1;
            }
            return ExpFloorTable.Cache[_lvl + 1].Guild;
        }

        public static Database.Models.ExpFloorModel GetNextFloor(int level)
        {
            if (ExpFloorTable.Cache.ContainsKey(level + 1))
            {
                return ExpFloorTable.Cache[level + 1];
            }
            else
            {
                return null;
            }

        }

        public static Database.Models.ExpFloorModel GetFloorByJobExp(int exp)
        {
            Database.Models.ExpFloorModel floor = null;
            foreach (var f in ExpFloorTable.Cache.Values)
            {
                if (f.Job <= exp)
                {
                    floor = f;
                }
            }
            return floor;

        }

        public static int MaxLevel
        {
            get
            {
                return Cache.Last().Key;
            }
        }
    }
}
