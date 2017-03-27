using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Database;
using Tera.WorldServer.Utils;
using Tera.Libs;

namespace Tera.WorldServer.Database
{
    public static class DatabaseManager
    {
        public static MySqlProvider Provider { get; set; }
        public static MySqlProvider RealmProvider { get; set; }

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

                RealmProvider = new MySqlProvider(
                            Settings.AppSettings.GetStringElement("Database.Host"),
                            Settings.AppSettings.GetStringElement("Database.rName"),
                            Settings.AppSettings.GetStringElement("Database.Username"),
                            Settings.AppSettings.GetStringElement("Database.Password"));
                RealmProvider.Connect();

                Logger.Info("Connected to the database !");
            }
            catch (Exception e)
            {
                Logger.Error("Can't connect to database : " + e.ToString());
            }
        }
    }
}
