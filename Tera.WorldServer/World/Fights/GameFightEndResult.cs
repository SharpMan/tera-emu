using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.World.Maps;

namespace Tera.WorldServer.World.Fights
{
    public sealed class GameFightEndResult
    {
        public Fight Fight { get; set; }
        public Dictionary<Fighter, Result> FightResults = new Dictionary<Fighter, Result>();
        public TaxCollectorResult TCollectorResult { get; set; }

        public GameFightEndResult(Fight Fight)
        {
            this.Fight = Fight;
        }

        public void AddResult(Fighter Fighter, bool Win, long WinKamas = 0, long WinExp = 0, long WinHonor = 0, long WinDisHonor = 0, long WinGuildXp = 0, long WinMountXp = 0, Dictionary<int, int> WinItems = null)
        {
            if (FightResults.ContainsKey(Fighter))
                return;
            var Result = new Result();

            Result.Fighter = Fighter;
            Result.Win = Win;
            Result.WinExp = WinExp;
            Result.WinHonor = WinHonor;
            Result.WinDisHonor = WinDisHonor;

            if (Win && WinExp > 0 && Fighter.ActorType == GameActorTypeEnum.TYPE_CHARACTER)
                Result.WinGuildXp = Algo.CalculateGuildXp((Fighter as CharacterFighter).Character, Result);
            if (Win && WinExp > 0 && Fighter.ActorType == GameActorTypeEnum.TYPE_CHARACTER && (Fighter as CharacterFighter).Character.isOnMount())
                 Result.WinMountXp = Algo.CalculateMountXp((Fighter as CharacterFighter).Character, Result);


            Result.WinItems = WinItems;
            Result.WinKamas = WinKamas;

            this.FightResults.Add(Fighter, Result);
        }

        public class Result
        {
            public Fighter Fighter { get; set; }
            public bool Win { get; set; }
            public long WinKamas { get; set; }
            public long WinExp { get; set; }
            public long WinHonor { get; set; }
            public long WinDisHonor { get; set; }
            public long WinGuildXp { get; set; }
            public long WinMountXp { get; set; }
            public Dictionary<int, int> WinItems { get; set; }
        }

        public class TaxCollectorResult
        {
            public TaxCollector TaxCollector { get; set; }
            public long WinExp { get; set; }
            public long WinKamas { get; set; }
            public Dictionary<int, int> WinItems { get; set; }
        }
    }
}
