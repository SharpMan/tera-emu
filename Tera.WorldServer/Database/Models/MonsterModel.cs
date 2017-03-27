using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Tera.Libs;
using Tera.Libs.Enumerations;
using Tera.WorldServer.World.Spells;

namespace Tera.WorldServer.Database.Models
{
    public class Monster
    {
        private List<MonsterLevel> myLevels = new List<MonsterLevel>(5);
        public List<Drop> DropsCache = new List<Drop>();

        public int ID { get; set; }
        public String Name { get; set; }
        public int Look { get; set; }
        public int Alignement { get; set; }
        public String Grades { get; set; }
        public String Colors { get; set; }
        public String Stats { get; set; }
        public String Spells { get; set; }
        public String Life { get; set; }
        public String Points { get; set; }
        public String Inits { get; set; }
        public int MinKamas { get; set; }
        public int MaxKamas { get; set; }
        public String Experiences { get; set; }
        public int AI_TYPE { get; set; }
        public int Capturable { get; set; }

        public string staticFighterEffects;

        public List<EffectInfos> StaticFighterBeginEffects
        {
            get;
            set;
        }


        public MonsterLevel GetLevel(int Level)
        {
            return myLevels.Find(x => x.Level == Level);
        }

        public MonsterLevel GetLevelOrNear(int Level)
        {
            int Near = 10000;
            MonsterLevel objNear = null;
            foreach (var objLevel in myLevels)
            {
                if (objLevel.Level == Level)
                {
                    return objLevel;
                }
                else
                {
                    int Diff = Math.Abs(objLevel.Level - Level);
                    if (Near > Diff)
                    {
                        Near = Diff;
                        objNear = objLevel;
                    }
                }
            }
            return objNear;
        }

        public List<MonsterLevel> GetMobs()
        {
            return this.myLevels;
        }

        private bool myInitialized = false;
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Initialize()
        {
            if (myInitialized)
                return;

           // this.DropsCache = new List<Drop>();
           // this.DropsCache.AddRange(this.Drops);

            try
            {
                var Grades = this.Grades.Split('|');
                var Stats = this.Stats.Split('|');
                var Spells = this.Spells.Split('|');
                var Lifes = this.Life.Split('|');
                var Initiatives = this.Inits.Split('|');
                var APMP = this.Points.Split('|');
                var Experiences = this.Experiences.Split('|');

                for (int i = 0; i < Grades.Length; i++)
                {
                    var Infos = Grades[i].Split('@');

                    // Statistique du grade
                    var GradeLevel = int.Parse(Infos[0]);
                    var GradeResistances = Infos[1];
                    var GradeStats = Stats[i];
                    var GradeSpells = Spells[i];
                    var GradeLife = int.Parse(Lifes[i]);
                    var GradeInitiative = int.Parse(Initiatives[i]);
                    var GradeAP = int.Parse(APMP[i].Split(';')[0]);
                    var GradeMP = int.Parse(APMP[i].Split(';')[1]);
                    var GradeBaseXP = long.Parse(Experiences[i]);

                    this.myLevels.Add
                    (
                        new MonsterLevel
                            (
                                this,
                                i + 1,
                                GradeLevel,
                                GradeLife,
                                GradeLife,
                                GradeInitiative,
                                GradeAP,
                                GradeMP,
                                GradeBaseXP,
                                GradeResistances,
                                GradeStats,
                                GradeSpells
                            )
                      );
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Monster::Intialize->Can't parse Mob::ID->"+this.ID);
            }
            try
            {
                if (this.staticFighterEffects != "-1")
                {
                    this.StaticFighterBeginEffects = new List<EffectInfos>();

                    if (this.staticFighterEffects.Contains('|'))
                    {
                        foreach (var Effect in this.staticFighterEffects.Split('|'))
                        {
                            var Data = Effect.Split(';');

                            if (Data.Length >= 4)
                            {
                                var Type = (EffectEnum)int.Parse(Data[0]);
                                var V1 = int.Parse(Data[1]);
                                var V2 = int.Parse(Data[2]);
                                var V3 = int.Parse(Data[3]);
                                var Duration = 0;
                                if (Data.Length > 4)
                                    Duration = int.Parse(Data[4]);
                                var Chance = 0;
                                if (Data.Length > 5)
                                    Chance = int.Parse(Data[5]);
                                var Range = "Pa";

                                this.StaticFighterBeginEffects.Add(new EffectInfos(null, Type, V1, V2, V3, Duration, Chance, Range));
                            }
                        }
                    }
                    else
                    {
                        var Data = this.staticFighterEffects.Split(';');

                        if (Data.Length >= 4)
                        {
                            var Type = (EffectEnum)int.Parse(Data[0]);
                            var V1 = int.Parse(Data[1]);
                            var V2 = int.Parse(Data[2]);
                            var V3 = int.Parse(Data[3]);
                            var Duration = 0;
                            if (Data.Length > 4)
                                Duration = int.Parse(Data[4]);
                            var Chance = 0;
                            if (Data.Length > 5)
                                Chance = int.Parse(Data[5]);
                            var Range = "Pa";

                            this.StaticFighterBeginEffects.Add(new EffectInfos(null, Type, V1, V2, V3, Duration, Chance, Range));
                        }
                    }
                }
            }
            catch (Exception e)
            {

            }

            myInitialized = true;
        }

        public void addDrop(Drop D)
        {
            this.DropsCache.Add(D);
        }
    }
}
