using MySql.Data.MySqlClient;
using System;
using System.Runtime.CompilerServices;

namespace Tera.Libs.Database
{
    public class MySqlProvider
    {
        public MySqlProvider(string host, string database, string username, string password)
        {
            string connectionString = string.Format("Server={0};Database={1};Uid={2};Pwd='{3}';",
                new object[] { host, database, username, password });
            this._connection = new MySqlConnection(connectionString);
            this._locker = new object();
        }

        public bool Connect()
        {
            try
            {
                this._connection.Open();
                return true;
            }
            catch (Exception exception)
            {
                Logger.Error(exception.ToString());
                return false;
            }
        }

        public bool ExecuteQuery(string query)
        {
            try
            {
                new MySqlCommand(query, this._connection).ExecuteNonQuery();
                return true;
            }
            catch (MySqlException e)
            {
                Logger.Error(e);
                this.Restart();
                return false;
            }
            catch (Exception exception)
            {
                Logger.Error(exception.ToString());
                return false;
            }
        }

        public bool ExecuteQuery(MySqlCommand Command)
        {
            try
            {
                Command.ExecuteNonQuery();
                return true;
            }
            catch (MySqlException e)
            {
                Logger.Error(e);
                this.Restart();
                return false;
            }
            catch (Exception exception)
            {
                Logger.Error(exception.ToString());
                return false;
            }
        }

        public MySqlDataReader ExecuteReader(string query)
        {
            try
            {
                MySqlCommand command = new MySqlCommand(query, this._connection);
                return command.ExecuteReader();
            }
            catch (MySqlException e)
            {
                Logger.Error(e);
                this.Restart();
                return null;
            }
            catch (Exception exception)
            {
                Logger.Error(exception.ToString());
                return null;
            }
        }

        public MySqlDataReader ExecuteCommand(MySqlCommand Command)
        {
            try
            {
                return Command.ExecuteReader();
            }
            catch (MySqlException e)
            {
                Logger.Error(e);
                this.Restart();
                return null;
            }
            catch (Exception exception)
            {
                Logger.Error(exception.ToString());
                return null;
            }
        }

        public long LastRestart;

        public void Restart()
        {
            /*if ((Environment.TickCount - LastRestart) < 1000 * 60 * 7)
            {
                Logger.Info("La base de donnée a deja redemarré y'a moin de 7min");
                return;
            }*/
            Logger.Error("Database Planted ! and Restarted ");
            this._connection.Close();
            this._connection.Open();
        }

        private MySqlConnection _connection { get; set; }

        public MySqlConnection getConnection()
        {
            return this._connection;
        }

        private object _locker { get; set; }

        public object GetLocker
        {
            get
            {
                return this._locker;
            }
        }
    }
}
