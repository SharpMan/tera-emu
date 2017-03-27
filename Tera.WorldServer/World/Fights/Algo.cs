using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Tera.Libs.Enumerations;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.Utils;
using Tera.WorldServer.World.Packets;
using Tera.Libs.Utils;
using Tera.WorldServer.World.Maps;
using Tera.Libs.Helper;

namespace Tera.WorldServer.World.Fights
{
    public static class Algo
    {
        private static Random r = new Random();

        public static int getRandomValue(int i1, int i2)
        {
            return r.Next(i2 - i1 + 1) + i1;
        }

        public static String newReceivedDate(String split)
        {
            var jour = DateTime.Now.ToString("dd");
            String moi = DateTime.Now.ToString("MM");
            String annee = DateTime.Now.ToString("yyyy");
            String heure= DateTime.Now.ToString("HH");
            String min= DateTime.Now.ToString("mm");
            int mois = int.Parse(moi) - 1;
            return (new StringBuilder(int.Parse(annee).ToString("x")).Append(split).Append(int.Parse(mois + "" + jour).ToString("x")).Append(split).Append(int.Parse(heure + "" + min).ToString("x"))).ToString(); 
	
        }

       

        public static int CalculateAggressionHonor(Fighter Fighter, bool Win, int WinnersCount, int WinnersTotalGrade, int WinnersTotalLevel, int LoosersTotalGrade, int LoosersTotalLevel)
        {
            if (WinnersTotalLevel - LoosersTotalLevel > 15)
                return 0;

            var Base = (int)(100 * (float)(LoosersTotalGrade / WinnersTotalGrade)) / WinnersCount;

            return Base; // TODO : Honor rate
        }

        public static long CalculatePVMXp(CharacterFighter Fighter, List<Fighter> Winners, List<Fighter> Loosers, int LoosersLevel, int WinnersLevel, long GrouXp)
        {
            int Level = Fighter.Level;
            var Sagesse = Fighter.Stats.GetTotal(EffectEnum.AddSagesse);
            var Coefficient = (Sagesse + 100) / 100;
            var Taux = 3;
            var LevelMax = Winners.Max(x => x.Level);
            double Bonus = 0;

            foreach (var Winner in Winners)
                if (Winner.Level > LevelMax / 3)
                    Bonus += 1;

            if (Bonus == 2)
            {
                Bonus = 1.1;
            }
            else if (Bonus == 3)
            {
                Bonus = 1.3;
            }
            else if (Bonus == 4)
            {
                Bonus = 2.2;
            }
            else if (Bonus == 5)
            {
                Bonus = 2.5;
            }
            else if (Bonus == 6)
            {
                Bonus = 2.8;
            }
            else if (Bonus == 7)
            {
                Bonus = 3.1;
            }
            else if (Bonus >= 8)
            {
                Bonus = 3.5;
            }

            double Rapport = 1 + ((double)LoosersLevel / (double)WinnersLevel);

            if (Rapport <= 1.3)
                Rapport = 1.3;

            if (Rapport > 5)
                Rapport = 5;

            double Rapport2 = 1 + ((double)Level / (double)WinnersLevel);

            return (long)(GrouXp * Rapport * Bonus * Taux * Coefficient * Rapport2);
        }

        public static long CalculateXpWinPerco(TaxCollector TaxCollector, List<Fighter> Winners, List<Fighter> Loosers, long GrouXp)
        {
            Guild G = TaxCollector.Guild;
            float sag = G.BaseStats.GetTotal(EffectEnum.AddSagesse);
            float coef = (sag + 100) / 100;
            int taux = Settings.AppSettings.GetIntElement("Rate.Pvm");
            long xpWin = 0;
            int lvlmax = 0;
            foreach (Fighter entry in Winners)
            {
                if (entry.Level > lvlmax)
                    lvlmax = entry.Level;
            }
            int nbbonus = 0;
            foreach (Fighter entry in Winners)
            {
                if (entry.Level > (lvlmax / 3))
                    nbbonus += 1;
            }

            double bonus = 1;
            if (nbbonus == 2)
                bonus = 1.1;
            if (nbbonus == 3)
                bonus = 1.3;
            if (nbbonus == 4)
                bonus = 2.2;
            if (nbbonus == 5)
                bonus = 2.5;
            if (nbbonus == 6)
                bonus = 2.8;
            if (nbbonus == 7)
                bonus = 3.1;
            if (nbbonus >= 8)
                bonus = 3.5;

            int lvlLoosers = 0;
            foreach (Fighter entry in Loosers)
                lvlLoosers += entry.Level;
            int lvlWinners = 0;
            foreach (Fighter entry in Winners)
                lvlWinners += entry.Level;
            double rapport = 1 + ((double)lvlLoosers / (double)lvlWinners);
            if (rapport <= 1.3)
                rapport = 1.3;
            /*
            if (rapport > 5)
                rapport = 5;
            //*/
            int lvl = G.Level;
            double rapport2 = 1 + ((double)lvl / (double)lvlWinners);

            xpWin = (long)(GrouXp * rapport * bonus * taux * coef * rapport2);

            return xpWin;
        }

        public static long CalculateGuildXp(Player Character, Tera.WorldServer.World.Fights.GameFightEndResult.Result result)
        {
            CharacterGuild gm = Character.getCharacterGuild();
            if (gm == null)
                return 0;

            double xp = (double)result.WinExp, Lvl = Character.Level, LvlGuild = Character.GetGuild().Level, pXpGive = (double)gm.ExperiencePercent / 100;

            double maxP = xp * pXpGive * 0.10;	//Le maximum donné à la guilde est 10% du montant prélevé sur l'xp du combat
            double diff = Math.Abs(Lvl - LvlGuild);	//Calcul l'écart entre le niveau du personnage et le niveau de la guilde
            double toGuild;
            if (diff >= 70)
            {
                toGuild = maxP * 0.10;	//Si l'écart entre les deux honor est de 70 ou plus, l'experience donnée a la guilde est de 10% la valeur maximum de don
            }
            else if (diff >= 31 && diff <= 69)
            {
                toGuild = maxP - ((maxP * 0.10) * (Math.Floor((diff + 30) / 10)));
            }
            else if (diff >= 10 && diff <= 30)
            {
                toGuild = maxP - ((maxP * 0.20) * (Math.Floor(diff / 10)));
            }
            else //Si la différence est [0,9]
            {
                toGuild = maxP;
            }
            result.WinExp = ((long)(xp - xp * pXpGive));
            long wonXP = (long)Math.Round(toGuild);

            if (wonXP > 0)
            {
                Character.getCharacterGuild().giveXpToGuild(wonXP);
            }

            return wonXP;
        }

        public static long CalculateMountXp(Player Character, Tera.WorldServer.World.Fights.GameFightEndResult.Result result)
        {
            if (Character.Mount == null)
            {
                return 0;
            }

            int diff = Math.Abs(Character.Level - Character.Mount.Level);

            double coeff = 0;
            double xp = (double)result.WinExp;
            double pToMount = (double)Character.MountXPGive / 100 + 0.2;

            if (diff >= 0 && diff <= 9)
            {
                coeff = 0.1;
            }
            else if (diff >= 10 && diff <= 19)
            {
                coeff = 0.08;
            }
            else if (diff >= 20 && diff <= 29)
            {
                coeff = 0.06;
            }
            else if (diff >= 30 && diff <= 39)
            {
                coeff = 0.04;
            }
            else if (diff >= 40 && diff <= 49)
            {
                coeff = 0.03;
            }
            else if (diff >= 50 && diff <= 59)
            {
                coeff = 0.02;
            }
            else if (diff >= 60 && diff <= 69)
            {
                coeff = 0.015;
            }
            else
            {
                coeff = 0.01;
            }

            if (pToMount > 0.2)
            {
                result.WinExp = ((long)(xp - (xp * (pToMount - 0.2))));
            }
            long xpWon = (long)Math.Round(xp * pToMount * coeff);

            if (xpWon > 0)
            {
                Character.Mount.addXp(xpWon);
                Character.Send(new CharacterRideEventMessage("+", Character.Mount));
            }

            return xpWon;
        }


        public static long CalculatePVMKamas(int MaxKamas, int MinKamas)
        {
            int rkamas = (int)(r.NextDouble() * (MaxKamas - MinKamas)) + MinKamas;
            return rkamas * Settings.AppSettings.GetIntElement("Rate.Kamas");
        }

        public static int Random(int Min, int Max)
        {
            return r.Next(Min, Max);
        }

        private static Couple<int, int> GetRandomBaseCellPlaces(Map map)
        {
            int team1_baseCell = map.getRandomWalkableCell();
            int team2_baseCell = map.getRandomWalkableCell();

            if (Pathfinder.GoalDistance(map, team1_baseCell, team2_baseCell) < 3)
            {
                return GetRandomBaseCellPlaces(map);
            }
            else
            {
                return new Couple<int, int>(team1_baseCell, team2_baseCell);
            }
        }

        public static Couple<List<FightCell>, List<FightCell>> GenRandomFightPlaces(Fight fight)
        {
            List<FightCell> team1 = new List<FightCell>();
            List<FightCell> team2 = new List<FightCell>();

            /*
             * BaseCells
             */
            Couple<int, int> baseCells = GetRandomBaseCellPlaces(fight.Map);
            team1.Add(fight.GetCell(baseCells.first));
            team2.Add(fight.GetCell(baseCells.second));

            /*
             * Remplissage
             */
            int boucles = 0;
            while (team1.Count < 8)
            {
                if (boucles > 500)
                {
                    break;
                }
                if (boucles > 25)
                {
                    int randomCellId = fight.Map.getRandomCell();
                    FightCell cell = fight.GetCell(randomCellId);
                    if (cell != null && cell.IsWalkable())
                    {
                        if (!team1.Contains(cell))
                        {
                            team1.Add(cell);
                        }
                    }
                    boucles++;
                    continue;
                }
                boucles++;
                FightCell toDir = team1[Random(0, team1.Count -1)];
                if (toDir == null)
                {
                    continue;
                }
                FightCell randomCell = fight.GetCell(Pathfinder.DirToCellID(toDir.Id, RandomDirection(), fight.Map, false));
                if (randomCell != null)
                {
                    if (!team1.Contains(randomCell) && randomCell.IsWalkable())
                    {
                        team1.Add(randomCell);
                    }
                }
            }

            boucles = 0;
            while (team2.Count < 8)
            {
                if (boucles > 500)
                {
                    break;
                }
                if (boucles > 25)
                {
                    int randomCellId = fight.Map.getRandomCell();
                    FightCell cell = fight.GetCell(randomCellId);
                    if (cell != null && cell.IsWalkable())
                    {
                        if (!team1.Contains(cell) && !team2.Contains(cell))
                        {
                            team2.Add(cell);
                        }
                    }
                    boucles++;
                    continue;
                }
                boucles++;
                FightCell toDir = team2[Random(0, team2.Count - 1)];
                if (toDir == null)
                {
                    continue;
                }
                FightCell randomCell = fight.GetCell(Pathfinder.DirToCellID(toDir.Id, RandomDirection(), fight.Map, false));
                if (randomCell != null)
                {
                    if (!team1.Contains(randomCell) && !team2.Contains(randomCell) && randomCell.IsWalkable())
                    {
                        team2.Add(randomCell);
                    }
                }
            }

            return new Couple<List<FightCell>, List<FightCell>>(team1, team2);
        }

        private static readonly char[] Directions = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h' };

        private static char RandomDirection()
        {
            return Directions[Random(0, Directions.Length - 1)];
        }

        public static string SerializeAsStartFightCells(Couple<List<FightCell>, List<FightCell>> cells)
        {
            StringBuilder s_cells = new StringBuilder();
            try
            {
                foreach (var cell in cells.first)
                {
                    int cellID = cell.Id;
                    int c1 = (cellID >> 6) << 6;
                    int c2 = cellID - c1;
                    s_cells.Append(StringHelper.IntToHash(c1 >> 6)).Append(StringHelper.IntToHash(c2));
                }
                s_cells.Append('|');
                foreach (var cell in cells.second)
                {
                    int cellID = cell.Id;
                    int c1 = (cellID >> 6) << 6;
                    int c2 = cellID - c1;
                    s_cells.Append(StringHelper.IntToHash(c1 >> 6)).Append(StringHelper.IntToHash(c2));
                }

                return s_cells.ToString();
            }
            catch (Exception e)
            {
                return "|";
            }
        }
    }
}
