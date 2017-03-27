using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace Tera.ORM
{
    public static class TeraProvider
    {
        public static TeraDatabase DbType = TeraDatabase.MYSQL;
        public static string Host { get; set; }
        public static string Username { get; set; }
        public static string Password { get; set; }
        public static string DB { get; set; }

        internal static MySqlConnection MYSQL_CONNECTION { get; set; }

        public static void Initialize(TeraDatabase dbtype, string host, string username, string password, string db)
        {
            DbType = dbtype;
            Host = host;
            Username = username;
            Password = password;
            DB = db;

            Connect();
        }

        public static void Connect()
        {
            switch (DbType)
            {
                case TeraDatabase.MYSQL:
                    MYSQL_CONNECTION = new MySqlConnection(string.Format("Server='{0}';Database='{1}';Uid='{2}';Pwd='{3}';", Host, DB, Username, Password));
                    MYSQL_CONNECTION.Open();
                    break;

                default:
                    throw new Exception("This dbtype is not supported yet !");
            }
        }
    }

    public enum TeraDatabase
    {
        MYSQL= 1,
        MSSQL = 2,
        SQLITE = 3,
        POSTGRESQL = 4,
        ORACLE = 5,
    }
    
}
