using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Realm.Database.Models;
using Tera.Libs;
using Tera.Realm.Database.Tables;

namespace Tera.Realm.Database
{
    public static class DatabaseCache
    {
        public static void Initialize()
        {
            DatabaseManager.Provider.ExecuteQuery("UPDATE accounts set logged = 0");
            Logger.Info(string.Format("{0} gameservers catched !", GameServerTable.FindAll()));
        }
    }
}
