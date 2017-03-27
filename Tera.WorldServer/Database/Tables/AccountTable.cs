using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;
using MySql.Data.MySqlClient;
using Tera.WorldServer.Database.Models;

namespace Tera.WorldServer.Database.Tables
{
    public class AccountTable
    {
        public static void UpdateLogged(int id, Boolean logged)
        {
            try
            {
                MySqlCommand Command = new MySqlCommand()
                {
                    Connection = DatabaseManager.RealmProvider.getConnection(),
                    CommandText = "UPDATE accounts SET logged=@stat WHERE guid=@guid",
                };
                Command.Prepare();
                Command.Parameters.AddWithValue("@guid", id);
                Command.Parameters.AddWithValue("@stat", logged ? (1) : (0));
                Command.ExecuteNonQuery();
            }
            catch (System.InvalidOperationException e1)
            {
                DatabaseManager.RealmProvider.Restart();
                UpdateLogged(id, logged);
            }
            catch (MySql.Data.MySqlClient.MySqlException e2)
            {
                DatabaseManager.RealmProvider.Restart();
                UpdateLogged(id, logged);
            }
            catch (Exception e3)
            {
                Logger.Error("Can't execute query : " + e3.ToString());
            }
        }

        public static void SubstractPoints(int id, int price)
        {
            try
            {
                MySqlCommand Command = new MySqlCommand()
                {
                    Connection = DatabaseManager.RealmProvider.getConnection(),
                    CommandText = "UPDATE accounts SET points = points - @price WHERE guid=@guid",
                };
                Command.Prepare();
                Command.Parameters.AddWithValue("@guid", id);
                Command.Parameters.AddWithValue("@price", price);
                Command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Logger.Error("Can't execute query : " + e.ToString());
            }
        }

        public static int getPoints(AccountModel account)
        {
            int cost = 0;
            try
            {
                var reader = DatabaseManager.RealmProvider.ExecuteReader("SELECT points FROM accounts WHERE guid = " + account.ID);
                if (reader.Read())
                {
                    cost = reader.GetInt32("points");
                }
                reader.Close();
            }
            catch (Exception e)
            {
                Logger.Error("Can't execute query : " + e.ToString());
                DatabaseManager.RealmProvider.Restart();
            }
            return cost;
        }

        public static void UpdateLogged(String Username, Boolean logged)
        {
            try
            {
                MySqlCommand Command = new MySqlCommand()
                {
                    Connection = DatabaseManager.RealmProvider.getConnection(),
                    CommandText = "UPDATE accounts SET logged=@stat WHERE username=@user",
                };
                Command.Prepare();
                Command.Parameters.AddWithValue("@user", Username);
                Command.Parameters.AddWithValue("@stat", logged ? (1) : (0));
                Command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Logger.Error("Can't execute query : " + e.ToString());
            }
        }

        public static void Update(AccountModel a)
        {
            try
            {
                MySqlCommand Command = new MySqlCommand()
                {
                    Connection = DatabaseManager.RealmProvider.getConnection(),
                    CommandText = "UPDATE accounts SET lastIP=@ip,lastConnectionDate=@lcd WHERE guid=@owner",
                };
                Command.Prepare();
                Command.Parameters.AddWithValue("@ip", a.LastIP);
                Command.Parameters.AddWithValue("@lcd", a.LastConnectionDate);
                Command.Parameters.AddWithValue("@owner", a.ID);
                Command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Logger.Error("Can't execute query : " + e.ToString());
            }
        }
    }
}
