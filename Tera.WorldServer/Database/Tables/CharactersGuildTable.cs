using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.Libs.Utils;
using Tera.WorldServer.Database.Models;

namespace Tera.WorldServer.Database.Tables
{
    public class CharactersGuildTable
    {
        public static void Load()
        {
            var reader = DatabaseManager.Provider.ExecuteReader("SELECT * FROM characters_guild");
            int br = 0;
            while (reader.Read())
            {
                var guild = GuildTable.GetGuild(reader.GetInt32("guild"));
                if (guild != null)
                {
                    var member = new Models.CharacterGuild()
                    {
                        ID = reader.GetInt64("guid"),
                        Name = reader.GetString("name"),
                        Guild = reader.GetInt32("guild"),
                        Level = reader.GetInt32("level"),
                        Gfx = reader.GetInt32("look"),
                        Grade = reader.GetInt32("rank"),
                        ExperiencePercent = reader.GetInt32("xpdone"),
                        Experience = reader.GetInt64("xpdone"),
                        Restriction = reader.GetInt32("rights"),
                        Alignement = reader.GetInt32("align"),
                        lastConnection = CDateTimeUtil.MySqlToNet(reader.GetMySqlDateTime("lastConnection")),
                    };
                    guild.CharactersGuildCache.Add(member);
                    br++;
                }
            }
            reader.Close();

            Logger.Info("Loaded @'" + br + "'@ Characters Guild");
        }

        public static void Delete(long id)
        {
            try
            {
                DatabaseManager.Provider.ExecuteQuery("DELETE FROM characters_guild WHERE guid = '" + id + "'");
            }
            catch (Exception e)
            {
                Logger.Error("Can't execute query : " + e.ToString());
            }
        }

        public static void Add(CharacterGuild member)
        {
            try
            {
                MySqlCommand Command = new MySqlCommand()
                {
                    Connection = DatabaseManager.Provider.getConnection(),
                    CommandText = "REPLACE INTO `characters_guild` VALUES(@guid,@guild,@name,@level,@look,@rank,@xpdone,@pxp,@rights,@align,@lastConnection);",
                };
                Command.Prepare();
                Command.Parameters.AddWithValue("@guid", member.ID);
                Command.Parameters.AddWithValue("@guild", member.Guild);
                Command.Parameters.AddWithValue("@name", member.Name);
                Command.Parameters.AddWithValue("@level", member.Level);
                Command.Parameters.AddWithValue("@look", member.Gfx);
                Command.Parameters.AddWithValue("@rank", member.Grade);
                Command.Parameters.AddWithValue("@xpdone", member.Experience);
                Command.Parameters.AddWithValue("@pxp", member.ExperiencePercent);
                Command.Parameters.AddWithValue("@rights", member.Restriction);
                Command.Parameters.AddWithValue("@align", member.Alignement);
                Command.Parameters.AddWithValue("@lastConnection", member.lastConnection);
                Command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Logger.Error("Can't execute query : " + e.ToString());
            }
        }

        public static int playerIsOnGuild(long guid)
        {
            //FIX There is already an open DataReader associated with this Connection which must be closed first
            try
            {
                return GuildTable.Cache.First(x => x.Value.CharactersGuildCache.First(y => y.ID == guid) != null).Key;
            }
            catch (Exception e)
            {
                return -1;
            }
            /*int guildId = -1;
            var reader = DatabaseManager.Provider.ExecuteReader("SELECT guild FROM characters_guild WHERE guid ="+guid);
            if (reader.Read())
            {
                guildId = reader.GetInt32("guild");
            }
            reader.Close();
            return guildId;*/
        }


    }
}
