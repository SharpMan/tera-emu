using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Realm.Utils;
using Tera.Libs;
using System.Data;
using Tera.Libs.Database;

namespace Tera.Realm.Database
{
    public static class DatabaseManager
    {
        public static MySqlProvider Provider { get; set; }
        internal static void Initialize()
        {
            try
            {
                Provider = new MySqlProvider(
                            Settings.AppSettings.GetStringElement("Database.Host"),
                            Settings.AppSettings.GetStringElement("Database.Name"),
                            Settings.AppSettings.GetStringElement("Database.Username"),
                            Settings.AppSettings.GetStringElement("Database.Password"));
                Provider.Connect();
                Logger.Info("Connected to the database !");
            }
            catch (Exception e)
            {
                Logger.Error("Can't connect to database : " + e.ToString());
            }
        }
    }
}
