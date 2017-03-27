using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.WorldServer.Database.Models;
using Tera.Libs;
using Tera.WorldServer.World.Actions;

namespace Tera.WorldServer.Database.Tables
{
    public class MapTable
    {
        public static Dictionary<short, Map> Cache = new Dictionary<short, Map>();
        public static string Collums = "id,date,width,heigth,places,key,mapData,cells,monsters,capabilities,mappos,numgroup,groupmaxsize";
        public static Random RANDOM = new Random();

        public static void Load()
        {
            long time = Environment.TickCount;
            var reader = DatabaseManager.Provider.ExecuteReader("SELECT * FROM maps");
            while (reader.Read())
            {
                var map = new Map()
                   {
                       Id = reader.GetInt16("id"),
                       Date = reader.GetString("date"),
                       Width = (byte)reader.GetInt32("width"),
                       Height = (byte)reader.GetInt32("heigth"),
                       FightCell = reader.GetString("places"),
                       Key = reader.GetString("key"),
                       maxGroup = (byte)reader.GetInt32("numgroup"),
                       GrouMaxSize = (byte)reader.GetInt32("groupmaxsize"),
                       mapData = reader.GetString("mapData"),
                       CellData = reader.GetString("cells"),
                       MapInfos = reader.GetString("mappos"),
                       Monsters = reader.GetString("monsters"),
                   };
                map.initPos();
                Cache.Add(map.Id, map);

            }
            reader.Close();
            Logger.Info("Loaded @'" + Cache.Count + "'@ maps in @" + (Environment.TickCount - time) + "@ ms");
        }

        public static void LoadActions()
        {
            long time = Environment.TickCount;
            var reader = DatabaseManager.Provider.ExecuteReader("SELECT * FROM endfight_action");
            int nbr = 0;
            while (reader.Read())
            {
                Map map = Get(reader.GetInt16("map"));
                if (map != null)
                {
                    map.addEndFightAction(reader.GetInt32("fighttype"),new ActionModel(reader.GetInt32("action"), reader.GetString("args"),reader.GetString("cond")));
                    nbr++;
                }

            }
            reader.Close();
            Logger.Info("Loaded @'" + nbr + "'@ EndFightActions in @" + (Environment.TickCount - time) + "@ ms");
        }

        public static Map Get(int mapid)
        {
            Map target = null;
            Cache.TryGetValue((short)mapid, out target);
            return target;
        }

        public static Map Get(short mapid)
        {
            Map target = null;
            Cache.TryGetValue(mapid, out target);
            return target;
        }


        public static Map GetRandomMap()
        {
            return Cache.Values.ToList()[RANDOM.Next(Cache.Count)];
        }
    }
}
