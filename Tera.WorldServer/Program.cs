using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.WorldServer.Utils;
using Tera.WorldServer.Database;
using Tera.WorldServer.World.Commands;
using Tera.WorldServer.World.Scripting;
using Tera.WorldServer.Constants;
using System.IO;
using Tera.WorldServer.World.Maps;
using Tera.Libs.Network;
using Tera.WorldServer.World.Commands.Player;

namespace Tera.WorldServer
{
    class Program
    {
        private static long startedCurrentTimeMillis;
        private static int UTCDiffTime;
        private static void StockcurrentTimeMillis()
        {
            TimeSpan tsUTC = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            TimeSpan tsNoUTC = (DateTimeOffset.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local));
            startedCurrentTimeMillis = (long)tsUTC.TotalMilliseconds - Environment.TickCount;
            UTCDiffTime = (int)(tsUTC.TotalMilliseconds - tsNoUTC.TotalMilliseconds);
        }
        public static long currentTimeMillis()
        {
            return startedCurrentTimeMillis + Environment.TickCount;
        }
        public static int getDiffUTCTime()
        {
            return (UTCDiffTime - (60*1000*60));
        }

        public static long StartTime = 0;

        static void Main(string[] args)
        {
            StockcurrentTimeMillis();

            Logger.Init();
            StartTime = Environment.TickCount;
            Settings.Initialize();
            Logger.Info("Settings loaded !");

            Logger.Stage("Database");
            DatabaseManager.Initialize();
            DatabaseCache.Initialize();

            Logger.Stage("Plugins");
            AdminCommandManager.Initialize();
            PlayerCommandManager.Initialize();
            ScriptKernel.Load();
            JSKernel.Load();

            Logger.Stage("Network");
            Network.InterClient.Initialize();
            Network.WorldServer.Initialize();

            Logger.Info("Tera started in " + (Environment.TickCount - StartTime) + "ms");
            
            while (true)
            {
                Console.ReadLine();
            }
        }
    }
}
