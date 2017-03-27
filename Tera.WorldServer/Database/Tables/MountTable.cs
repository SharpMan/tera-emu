using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.WorldServer.Database.Models;

namespace Tera.WorldServer.Database.Tables
{
    public static class MountTable
    {
        public static Dictionary<int, Mount> Cache = new Dictionary<int, Mount>();

        public static void Load()
        {
            var reader = DatabaseManager.Provider.ExecuteReader("SELECT * FROM mounts_data");
            while (!reader.IsClosed &&reader.Read())
            {
                var mount = new Mount()
                {
                    ID = reader.GetInt32("id"),
                    Color = reader.GetInt32("color"),
                    Sexe = reader.GetInt32("sexe"),
                    Amour = reader.GetInt32("amour"),
                    Endurance = reader.GetInt32("endurance"),
                    Level = reader.GetInt32("level"),
                    Exp = reader.GetInt64("xp"),
                    Name = reader.GetString("name"),
                    Fatigue = reader.GetInt32("fatigue"),
                    Energy = reader.GetInt32("energie"),
                    Reproduction = reader.GetInt32("reproductions"),
                    Maturite = reader.GetInt32("maturite"),
                    Serenite = reader.GetInt32("serenite"),
                    itemList = reader.GetString("items"),
                    Ancestres  = reader.GetString("ancetres"),
                };
                        
                Cache.Add(mount.ID, mount);
            }
            reader.Close();
            Logger.Info("Loaded @'" + Cache.Count + "'@ Character Mount");
        }

        public static void Update(Mount item)
        {
            try
            {
                MySqlCommand Command = new MySqlCommand()
                {
                    Connection = DatabaseManager.Provider.getConnection(),
                    CommandText = "UPDATE `mounts_data` set name = @name, xp = @xp, level = @level, endurance = @endurance, amour = @amour, maturite = @maturite, serenite = @serenite, reproductions = @reproductions, fatigue = @fatigue, energie = @energie, items = @items, ancetres = @ancetres WHERE id= @id;",
                };
                Command.Prepare();
                Command.Parameters.AddWithValue("@name", item.Name);
                Command.Parameters.AddWithValue("@xp", item.Exp);
                Command.Parameters.AddWithValue("@level", item.Level);
                Command.Parameters.AddWithValue("@endurance", item.Endurance);
                Command.Parameters.AddWithValue("@amour", item.Amour);
                Command.Parameters.AddWithValue("@maturite", item.Maturite);
                Command.Parameters.AddWithValue("@serenite", item.Serenite);
                Command.Parameters.AddWithValue("@reproductions", item.Reproduction);
                Command.Parameters.AddWithValue("@fatigue", item.Fatigue);
                Command.Parameters.AddWithValue("@energie", item.Energy);
                Command.Parameters.AddWithValue("@items", item.getItemsId());
                Command.Parameters.AddWithValue("@ancetres", item.Ancestres);
                Command.Parameters.AddWithValue("@id", item.ID);
                
                Command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        public static void Add(Mount item)
        {
            try
            {
                MySqlCommand Command = new MySqlCommand()
                {
                    Connection = DatabaseManager.Provider.getConnection(),
                    CommandText = "REPLACE INTO `mounts_data` VALUES(@guid,@color,@sexe,@name,@xp,@level,@endurance,@amour,@maturite,@serenite,@reproductions,@fatigue,@energie,@items,@ancetres);",
                };
                Command.Prepare();
                Command.Parameters.AddWithValue("@guid", item.ID);
                Command.Parameters.AddWithValue("@color", item.Color);
                Command.Parameters.AddWithValue("@sexe", item.Sexe);
                Command.Parameters.AddWithValue("@name", item.Name);
                Command.Parameters.AddWithValue("@xp", item.Exp);
                Command.Parameters.AddWithValue("@level", item.Level);
                Command.Parameters.AddWithValue("@endurance", item.Endurance);
                Command.Parameters.AddWithValue("@amour", item.Amour);
                Command.Parameters.AddWithValue("@maturite", item.Maturite);
                Command.Parameters.AddWithValue("@serenite", item.Serenite);
                Command.Parameters.AddWithValue("@reproductions", item.Reproduction);
                Command.Parameters.AddWithValue("@fatigue", item.Fatigue);
                Command.Parameters.AddWithValue("@energie", item.Energy);
                Command.Parameters.AddWithValue("@items", item.getItemsId());
                Command.Parameters.AddWithValue("@ancetres", item.Ancestres);

                Command.ExecuteNonQuery();
                Cache.Add(item.ID, item);
            }
            catch (Exception e)
            {
                Logger.Error("Can't execute query : " + e.ToString());
            }
        }

        public static int getGeneration(int color)
        {
            switch (color)
            {
                case 10: 
                case 18: 
                case 20: 
                    return 1;
                case 33: 
                case 38: 
                case 46: 
                    return 2;
                case 3: 
                case 17: 
                    return 3;
                case 62:
                case 12: 
                case 36: 
                case 34: 
                case 44: 
                case 42: 
                case 51: 
                    return 4;
                case 19:
                case 22: 
                    return 5;
                case 71: 
                case 70: 
                case 41: 
                case 40: 
                case 49: 
                case 48: 
                case 65: 
                case 64: 
                case 54: 
                case 53: 
                case 76: 
                    return 6;
                case 15: 
                case 16: 
                    return 7;
                case 11: 
                case 69: 
                case 37: 
                case 39: 
                case 45: 
                case 47: 
                case 61: 
                case 63: 
                case 9: 
                case 52: 
                case 68: 
                case 73: 
                case 67: 
                case 72: 
                case 66:
                    return 8;
                case 21: 
                case 23:
                    return 9;
                case 57:
                case 35: 
                case 43:
                case 50: 
                case 55: 
                case 56: 
                case 58: 
                case 59: 
                case 60: 
                case 77: 
                case 78:
                case 79: 
                case 80:
                case 82: 
                case 83:
                case 84: 
                case 85: 
                case 86: 
                    return 10;
                default:
                    return 1;
            }
        }

        public static int getNextGuid()
        {
            var reader = DatabaseManager.Provider.ExecuteReader("SELECT MAX(id) AS max FROM mounts_data");
            int id = 0;
            if (reader.Read())
            {
                try
                {
                    id = reader.GetInt32("max");
                }
                catch (Exception e) { }
            }
            reader.Close();
            return id;
        }
        public static Mount getMount(int p)
        {
            if (Cache.ContainsKey(p))
            {
                return Cache[p];
            }
            return null;
        }
    }
}
