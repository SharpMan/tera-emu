using System;
using System.Collections.Generic;
using System.Text;
using Tera.Libs;
using Tera.Realm.Utils;
using Tera.Realm.Network;
using Tera.Realm.Database;

namespace Tera.Realm
{
    public class Program
    {
        public static long  StartTime = 0;

        static void Main(string[] args)
        {
            Logger.Init();
            StartTime = Environment.TickCount;
            Settings.Initialize();
            Logger.Info("Settings loaded !");
            Logger.Stage("Database");
            DatabaseManager.Initialize();
            DatabaseCache.Initialize();
            Logger.Stage("Network");
            LoginServer.Initialize();
            InterServer.Initialize();
            Logger.Info("Tera started in " + (Environment.TickCount - StartTime) + "ms");

            while (true)
            {
                Console.ReadLine();
            }
        }
    }
}
