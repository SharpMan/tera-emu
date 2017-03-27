using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;

namespace Tera.Realm.Database.Tables
{
    public class GameServerTable
    {
        public static List<Models.GameServerModel> Cache = new List<Models.GameServerModel>();

        public static int FindAll()
        {
                Cache.Clear();
                var reader = DatabaseManager.Provider.ExecuteReader("SELECT * FROM realmlist");
                while (reader.Read())
                {
                    var gameserver = new Models.GameServerModel()
                    {
                        ID = reader.GetInt32("ID"),
                        Adress = reader.GetString("address"),
                        Port = reader.GetInt32("port"),
                        PlayerLimit = reader.GetInt32("player_limit"),
                        LevelRequired = reader.GetInt32("allowedSecurityLevel"),
                        Key = reader.GetString("key"),
                    };
                    Cache.Add(gameserver);
                }
                reader.Close();
                return Cache.Count;
        }
    
    }
}
