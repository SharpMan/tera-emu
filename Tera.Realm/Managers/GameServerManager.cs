using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Enumerations;
using Tera.Realm.Database.Tables;

namespace Tera.Realm.Managers
{
    public static class GameServerManager
    {
        public static Boolean hasOpenedServers()
        {
            lock (GameServerTable.Cache)
            {
                var Servers = GameServerTable.Cache.FindAll(x => x.State > ServerStateEnum.Offline);
                return Servers != null && Servers.Count > 0;
            }
        }
    }
}
