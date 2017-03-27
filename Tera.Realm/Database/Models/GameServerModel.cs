using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Enumerations;

namespace Tera.Realm.Database.Models
{
    public class GameServerModel
    {
        public int ID;
        public String Adress;
        public int Port;
        public int PlayerLimit;
        public int LevelRequired;
        public String Key;

        public ServerStateEnum State = ServerStateEnum.Offline;
        public static int Completion = 110;
    }
}
