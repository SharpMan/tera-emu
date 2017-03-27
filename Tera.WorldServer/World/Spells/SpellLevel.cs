using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.Libs.Enumerations;
using Tera.Libs.Utils;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.World.Fights;
using Tera.WorldServer.World.Maps;
using System.Runtime.CompilerServices;

namespace Tera.WorldServer.World.Spells
{
    public class SpellLevel
    {
        private bool myInitialized = false;

        public int Level { get; set; }
        public string Data { get; set; }

        public int APCost { get; set; }
        public int MinPO { get; set; }
        public int MaxPO { get; set; }
        public int TauxCC { get; set; }
        public int TauxEC { get; set; }
        public bool InLine { get; set; }
        public bool NeedVisibility { get; set; }
        public bool NeedEmptyCell { get; set; }
        public bool AllowPOBoost { get; set; }
        public int MaxPerTurn { get; set; }
        public int MaxPerPlayer { get; set; }
        public int TurnNumber { get; set; }
        public string Range { get; set; }
        public string StringEffects { get; set; }
        public string StringCriticEffects { get; set; }
        public bool IsECEndTurn { get; set; }

        public SpellLevel(int level, string data, SpellModel engine)
        {
            this.Level = level;
            this.Data = data;
            this.SpellCache = engine;
            this.LoadLevel();
        }

        public void LoadLevel()
        {
            try
            {
                if (this.Data == "-1" || this.Data == "")
                    return;
                string[] data = this.Data.Split(',');

                StringEffects = data[0];
                StringCriticEffects = data[1];

                this.APCost = 6;
                if (TypesManager.IsNumeric(data[2]))
                {
                    this.APCost = int.Parse(data[2]);
                }

                this.MinPO = int.Parse(data[3]);
                this.MaxPO = int.Parse(data[4]);
                this.TauxCC = int.Parse(data[5]);
                this.TauxEC = int.Parse(data[6]);

                this.InLine = bool.Parse(data[7].Trim());
                this.NeedVisibility = bool.Parse(data[8].Trim());
                this.NeedEmptyCell = bool.Parse(data[9].Trim());
                this.AllowPOBoost = bool.Parse(data[10].Trim());

                this.MaxPerTurn = int.Parse(data[12]);
                this.MaxPerPlayer = int.Parse(data[13]);
                this.TurnNumber = int.Parse(data[14]);
                this.Range = data[15].Trim();
                try
                {
                    this.IsECEndTurn = bool.Parse(data[19].Trim());
                }
                catch (Exception e)
                {
                }
            }
            catch (Exception e)
            {
                Logger.Error("Cant load level '" + this.Level + "' for spell '" + this.SpellCache.Name + "'" + e.ToString());
            }
        }


        public List<EffectInfos> Effects
        {
            get;
            set;
        }

        public List<EffectInfos> CriticEffects
        {
            get;
            set;
        }

        public SpellModel SpellCache { get; set; }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Initialize()
        {
            if (this.myInitialized)
                return;

            var Index = 0;

            if (this.StringEffects != "-1")
            {
                this.Effects = new List<EffectInfos>();

                if (this.StringEffects.Contains('|'))
                {
                    foreach (var Effect in this.StringEffects.Split('|'))
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
                            var Range = this.Range.Trim().Substring(Index, 2);

                            this.Effects.Add(new EffectInfos(this, Type, V1, V2, V3, Duration, Chance, Range));

                            Index += 2;
                        }
                    }
                }
                else
                {
                    var Data = this.StringEffects.Split(';');

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
                        var Range = this.Range.Trim().Substring(Index, 2);

                        this.Effects.Add(new EffectInfos(this, Type, V1, V2, V3, Duration, Chance, Range));

                        Index += 2;
                    }
                }
            }
            if (this.StringCriticEffects != "-1")
            {
                this.CriticEffects = new List<EffectInfos>();

                if (this.StringCriticEffects.Contains('|'))
                {
                    foreach (var Effect in this.StringCriticEffects.Split('|'))
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
                            var Range = this.Range.Trim().Substring(Index, 2);

                            this.CriticEffects.Add(new EffectInfos(this, Type, V1, V2, V3, Duration, Chance, Range));

                            Index += 2;
                        }
                    }
                }
                else
                {
                    var Data = this.StringCriticEffects.Split(';');

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
                        var Range = this.Range.Trim().Substring(Index, 2);

                        this.CriticEffects.Add(new EffectInfos(this, Type, V1, V2, V3, Duration, Chance, Range));

                        Index += 2;
                    }
                }
            }
            this.myInitialized = true;
        }

        public static List<Fighter> getTargets(EffectInfos SE, Fight fight, List<FightCell> cells)
        {
            List<Fighter> cibles = new List<Fighter>();
            foreach (FightCell aCell in cells)
            {
                if (aCell == null)
                {
                    continue;
                }
                Fighter f = aCell.GetObjects<Fighter>().First();
                if (f == null)
                {
                    continue;
                }
                cibles.Add(f);
            }
            return cibles;
        }
    }
}
