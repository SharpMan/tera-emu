using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Realm.Database.Models;
using MySql.Data.MySqlClient;
using Tera.Libs;
using Tera.Libs.Utils;
using Tera.Libs.Utils;

namespace Tera.Realm.Database.Tables
{
    public class AccountTable
    {
        public static AccountModel FindFirst(string username)
        {
            AccountModel account = null;
            MySqlDataReader reader = null;
            try
            {
                
                MySqlCommand Command = new MySqlCommand()
                {
                    Connection = DatabaseManager.Provider.getConnection(),
                    CommandText =
"SELECT guid,username,pass,banned,level,pseudo,logged,question,answer,lastip,lastConnectionDate FROM accounts WHERE username=@user",
                };
                Command.Prepare();
                Command.Parameters.AddWithValue("@user", username);

                reader = DatabaseManager.Provider.ExecuteCommand(Command);
                if (reader.Read())
                {
                    account = new Models.AccountModel()
                    {
                        ID = reader.GetInt32("guid"),
                        Username = reader.GetString("username"),
                        Password = reader.GetString("pass"),
                        Pseudo = reader.GetString("pseudo"),
                        Level = reader.GetInt32("level"),
                        Banned = reader.GetInt64("banned"),
                        Logged = reader.GetInt32("logged"),
                        LastIP = reader.GetString("lastip"),
                        SecretQuestion = reader.GetString("question"),
                        SecretAnswer = reader.GetString("answer"),
                        LastConnectionDate = CDateTimeUtil.MySqlToNet(reader.GetMySqlDateTime("lastConnectionDate")),
                    };
                    
                }
                reader.Close();
                if (account != null) FindCharacters(account);
                Command = null;

            }
            catch (Exception e)
            {
                if (reader != null && !reader.IsClosed)
                    reader.Close();
                Logger.Error(e);
            }
            return account;
          }

        public static void FindCharacters(AccountModel account)
        {
            MySqlCommand Command = new MySqlCommand()
            {
                Connection = DatabaseManager.Provider.getConnection(),
                CommandText = "SELECT server_id,guid FROM characters WHERE owner=@owner",
            };
            Command.Prepare();
            Command.Parameters.AddWithValue("@owner", account.ID);
            var reader = DatabaseManager.Provider.ExecuteCommand(Command);

            account.Characters = new Dictionary<long, int>();
            while (reader.Read())
            {
                account.Characters.Add(reader.GetInt64("guid"), reader.GetInt32("server_id"));
            }
            reader.Close();
            Command = null;
        }
        
    }
}
