using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.Network;
using Tera.WorldServer.Utils;

namespace Tera.WorldServer.Database.Tables
{
    public class GuildTable
    {
        public static Dictionary<int, Guild> Cache = new Dictionary<int, Guild>();

        public static void Load()
        {
            Cache.Clear();
            var reader = DatabaseManager.Provider.ExecuteReader("SELECT * FROM guilds");
            while (reader.Read())
            {
                var guild = new Models.Guild()
                {
                    ID = reader.GetInt32("id"),
                    Name = reader.GetString("name"),
                    Emblem = reader.GetString("emblem"),
                    Level = reader.GetInt32("level"),
                    Experience = reader.GetInt64("exp"),
                    Capital = reader.GetInt32("capital"),
                    Spells = reader.GetString("spells"),
                    Stats = reader.GetString("stats"),
                    PerceptorMaxCount = reader.GetInt32("maxperco"),
                };
                Cache.Add(guild.ID, guild);
            }
            reader.Close();

            Logger.Info("Loaded @'" + Cache.Count + "'@ Guilds");
        }

        public static Boolean Contains(String Name)
        {
            return Cache.Values.FirstOrDefault(x => x.Name.Trim().ToLower() == Name.Trim().ToLower()) != null;
        }

        public static int getNextGuid()
        {
            var reader = DatabaseManager.Provider.ExecuteReader("SELECT MAX(id) AS max FROM guilds");
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

        public static void TryDeleteGuild(Guild Guild)
        {
            try
            {
                lock (Cache)
                    Cache.Remove(Guild.ID);

                DatabaseManager.Provider.ExecuteQuery("DELETE FROM guilds WHERE id = '" + Guild.ID + "'");

            }
            catch (Exception ex)
            {
                Logger.Error("DatabaseEntities::TryDeleteGuild : Name=" + Guild.Name + " " + ex.ToString());
            }
        }

        public static Guild TryCreateGuild(WorldClient Client, string Name, string Emblem)
        {
            try
            {

                var Guild = new Guild()
                {
                    ID = DatabaseCache.nextGuildId++,
                    Name = Name,
                    Emblem = Emblem,
                    Level = 1,
                    Experience = 0,
                    Capital = 0,
                    Spells = "462;0;25,461;0;25,460;0;25,459;0;25,458;0;25,457;0;25,456;0;25,455;0;25,454;0;25,453;0;25,452;0;25,451;0;25,",
                    Stats = StaticData.GUILD_BASE_STATS,
                    PerceptorMaxCount = 0,
                };

                Guild.Initialize();

                Add(Guild);

                return Guild;
            }
            catch (Exception ex)
            {
                Logger.Error("DatabaseEntities::TryCreateGuild : Name=" + Name + " Emblem=" + Emblem + " " + ex.ToString());
                return null;
            }
        }

        public static void Update(Guild item)
        {
            try
            {
                MySqlCommand Command = new MySqlCommand()
                {
                    Connection = DatabaseManager.Provider.getConnection(),
                    CommandText = "UPDATE `guilds` set level = @level, exp = @exp, capital = @capital, maxperco = @maxperco, spells = @spells, stats = @stats WHERE id= @id;",
                };
                Command.Prepare();
                Command.Parameters.AddWithValue("@id", item.ID);
                Command.Parameters.AddWithValue("@level", item.Level);
                Command.Parameters.AddWithValue("@exp", item.Experience);
                Command.Parameters.AddWithValue("@capital", item.Capital);
                Command.Parameters.AddWithValue("@maxperco", item.PerceptorMaxCount);
                Command.Parameters.AddWithValue("@spells", item.Spells);
                Command.Parameters.AddWithValue("@stats", item.Stats);
                Command.ExecuteNonQuery();
            }
            catch (System.InvalidOperationException e1)
            {
                DatabaseManager.Provider.Restart();
                Update(item);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        public static void Add(Guild item)
        {
            try
            {
                MySqlCommand Command = new MySqlCommand()
                {
                    Connection = DatabaseManager.Provider.getConnection(),
                    CommandText = "INSERT INTO `guilds` VALUES(@guid,@name,@emblem,@level,@exp,@capital,@spells,@stats,@maxperco);",
                };
                Command.Prepare();
                Command.Parameters.AddWithValue("@guid", item.ID);
                Command.Parameters.AddWithValue("@name", item.Name);
                Command.Parameters.AddWithValue("@emblem", item.Emblem);
                Command.Parameters.AddWithValue("@level", item.Level);
                Command.Parameters.AddWithValue("@exp", item.Experience);
                Command.Parameters.AddWithValue("@capital", item.Capital);
                if (item.GetSpellBook() != null)
                {
                    Command.Parameters.AddWithValue("@spells", item.GetSpellBook().ToDatabase());
                }
                else
                {
                    Command.Parameters.AddWithValue("@spells", "");
                }
                Command.Parameters.AddWithValue("@stats", item.Stats);
                Command.Parameters.AddWithValue("@maxperco", item.PerceptorMaxCount);
                Command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Logger.Error("Can't execute query : " + e.ToString());
            }
        }

        public static void addItem(Guild guild, bool saveSQL)
        {
            lock (Cache)
                Cache.Add(guild.ID, guild);
            if (saveSQL)
            {
                Add(guild);
            }
        }

        public static Guild GetGuild(int id)
        {
            if (Cache.ContainsKey(id))
            {
                return Cache[id];
            }
            else
            {
                return null;
            }

        }

    }
}
