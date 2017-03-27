using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Realm.Network;

namespace Tera.Realm.Managers
{
    static public class AccountManager
    {
        public static Boolean isConnectedToRealm(String account)
        {
            var compte = LoginServer.Clients.FindAll(x => x.Account != null && x.Account.Username == account);
            return compte.Count != 0;
        }

        public static void KickWithAccont(String account)
        {
             LoginServer.Clients.FindAll(x => x.Account != null && x.Account.Username == account).ForEach(x => x.Disconnect());
        }
    }
}
