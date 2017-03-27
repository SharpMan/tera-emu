using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.WorldServer.Database.Models;

namespace Tera.WorldServer.Database.Tables
{
    public static class AccountDataTable
    {
        public static AccountDataModel Get(int id)
        {
            AccountDataModel model = null;
            var reader = DatabaseManager.Provider.ExecuteReader("SELECT * FROM accounts_data WHERE guid = " + id);
            if (reader.Read())
            {
                model = new AccountDataModel()
                {
                    Guid = reader.GetInt32("guid"),
                    Bank = reader.GetString("bank"),
                    BankKamas = reader.GetInt64("bankkamas"),
                    Stables = reader.GetString("stables"),
                    Friends = reader.GetString("friends"),
                    Ennemys = reader.GetString("enemys"),
                    showFriendConnection = reader.GetInt32("seeFriend") == 1,
                };
            }
            reader.Close();
            return model;
        }

        public static void Update(AccountDataModel model)
        {
            try
            {
                MySqlCommand Command = new MySqlCommand()
                {
                    Connection = DatabaseManager.Provider.getConnection(),
                   // CommandText = "UPDATE `accounts_data` VALUES(@client,@bank,@kamas,@stable,@friend,@ennemy,@seefriend);",
                    CommandText = "UPDATE `accounts_data` set bank = @bank, bankkamas = @kamas, stables = @stable , friends = @friend , enemys = @ennemy , seeFriend = @seefriend  WHERE guid= @guid;",
                };
                Command.Prepare();
                Command.Parameters.AddWithValue("@guid", model.Guid);
                Command.Parameters.AddWithValue("@bank", model.Bank);
                Command.Parameters.AddWithValue("@kamas", model.BankKamas);
                Command.Parameters.AddWithValue("@stable", model.Stables);
                Command.Parameters.AddWithValue("@friend", model.Friends);
                Command.Parameters.AddWithValue("@ennemy", model.Ennemys);
                Command.Parameters.AddWithValue("@seefriend", (model.showFriendConnection ? 1 : 0));
                Command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Logger.Error("Can't execute query : " + e.ToString());
            }
        }

        public static void Update(long count,long id)
        {
            try
            {
                MySqlCommand Command = new MySqlCommand()
                {
                    Connection = DatabaseManager.Provider.getConnection(),
                    // CommandText = "UPDATE `accounts_data` VALUES(@client,@bank,@kamas,@stable,@friend,@ennemy,@seefriend);",
                    CommandText = "UPDATE accounts_data SET bankkamas = bankkamas + @kamas WHERE guid=(SELECT owner from characters WHERE guid = @id LIMIT 1) ;",
                };
                Command.Prepare();
                Command.Parameters.AddWithValue("@kamas", count);
                Command.Parameters.AddWithValue("@id", id);
                Command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Logger.Error("Can't execute query : " + e.ToString());
            }
        }

        public static void UpdateKamas(long kamas, int id)
        {
            try
            {
                MySqlCommand Command = new MySqlCommand()
                {
                    Connection = DatabaseManager.Provider.getConnection(),
                    // CommandText = "UPDATE `accounts_data` VALUES(@client,@bank,@kamas,@stable,@friend,@ennemy,@seefriend);",
                    CommandText = "UPDATE accounts_data SET bankkamas = bankkamas + @kamas WHERE guid=@id;",
                };
                Command.Prepare();
                Command.Parameters.AddWithValue("@kamas", kamas);
                Command.Parameters.AddWithValue("@id", id);
                Command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Logger.Error("Can't execute query : " + e.ToString());
            }
        }

        public static void Add(AccountDataModel model)
        {
            try
            {
                MySqlCommand Command = new MySqlCommand()
                {
                    Connection = DatabaseManager.Provider.getConnection(),
                    CommandText = "REPLACE INTO `accounts_data` VALUES(@guid,@bank,@kamas,@stable,@friend,@ennemy,@seefriend);",
                };
                Command.Prepare();
                Command.Parameters.AddWithValue("@guid", model.Guid);
                Command.Parameters.AddWithValue("@bank", model.Bank);
                Command.Parameters.AddWithValue("@kamas", model.BankKamas);
                Command.Parameters.AddWithValue("@stable", model.Stables);
                Command.Parameters.AddWithValue("@friend", model.Friends);
                Command.Parameters.AddWithValue("@ennemy", model.Ennemys);
                Command.Parameters.AddWithValue("@seefriend", (model.showFriendConnection ? 1 : 0));
                Command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Logger.Error("Can't execute query : " + e.ToString());
            }
        }
    }
}
