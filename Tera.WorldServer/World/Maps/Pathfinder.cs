using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.Libs.Enumerations;
using Tera.Libs.Helper;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.World.Fights;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.World.Maps
{
    public static class Pathfinder
    {
        public struct Point
        {
            public double X;
            public double Y;
            public double Z;
            public int CellId;

            public Point(int CellId, double x, double y)
            {
                this.CellId = CellId;
                this.X = x;
                this.Y = y;
                this.Z = 0;
            }

            public Point(Point Base)
            {
                this.CellId = Base.CellId;
                this.X = Base.X;
                this.Y = Base.Y;
                this.Z = Base.Z;
            }
        }
        public const double RUN_SPEED = 0.20;
        public const double WALK_SPEED = 0.50;
        private static int[] FIGHT_DIRECTIONS = { 1, 3, 5, 7 };
        private static Random PATHFIND_RANDOM = new Random();
        private static Dictionary<int, Dictionary<int, Point>> myCellToPointGrid = new Dictionary<int, Dictionary<int, Point>>();


        public static MovementPath IsValidPath(Map Map, int CurrentCell, string EncodedPath)
        {
            var DecodedPath = Pathfinder.DecodePath(Map, CurrentCell, EncodedPath);

            var Index = 0;
            int TransitCell = 0;
            do
            {
                TransitCell = DecodedPath.TransitCells[Index];

                var Length = Pathfinder.IsValidLine(Map, TransitCell, DecodedPath.GetDirection(TransitCell), DecodedPath.TransitCells[Index + 1]);
                if (Length == -1)
                    return null;

                DecodedPath.MovementLength += Length;

                Index++;

            }
            while (TransitCell != DecodedPath.LastStep);

            return DecodedPath;
        }

        public static Boolean isNextTo(int cell1, int cell2)
        {
            return (cell1 + 14 == cell2) || (cell1 + 15 == cell2) || (cell1 - 14 == cell2) || (cell1 - 15 == cell2);
        }

        public static string CreateStringPath(int baseCell, int baseDir, List<int> cells, Map map)
        {
            string path = GetDirChar(baseDir) + GetCellChars(baseCell);
            foreach (int cell in cells)
            {
                path += GetDirChar(Pathfinder.GetDirection(map, baseCell, cell)) + GetCellChars(cell);
                baseCell = cell;
            }
            return path;
        }

        public static int getNearestCellAround(Map map, int startCell, int endCell, List<DofusCell> forbidens, Fight fight)
        {
            try
            {
                int dist = 1000;
                int cellID = startCell;
                if (forbidens == null)
                {
                    forbidens = new List<DofusCell>();
                }
                char[] dirs = { 'b', 'd', 'f', 'h' };

                foreach (char d in dirs)
                {
                    int c = DirToCellID(startCell, d, map, true);
                    int dis = GetDistanceBetween(map, endCell, c);

                    if ((dis >= dist)
                            || (!fight.GetCell(c).CanWalk())
                            || (forbidens.Contains(map.getCell(c))))
                    {
                        continue;
                    }
                    dist = dis;
                    cellID = c;
                }

                return cellID == startCell ? -1 : cellID;
            }
            catch (Exception e)
            {
            }
            return 0;
        }

        public static List<DofusCell> getShortestPathBetween(Map map, int start, int dest, int distMax,Fight fight)
        {
            try
            {
                List<DofusCell> curPath = new List<DofusCell>();
                List<DofusCell> curPath2 = new List<DofusCell>();
                List<DofusCell> closeCells = new List<DofusCell>();
                int limit = 1000;

                DofusCell curCase = map.getCell(start);
                int stepNum = 0;
                Boolean stop = false;

                while ((!stop) && (stepNum++ <= limit))
                {
                    int nearestCell = getNearestCellAround(map, curCase.Id, dest, closeCells,fight);
                    if (nearestCell == -1)
                    {
                        closeCells.Add(curCase);
                        if (curPath.Count > 0)
                        {
                            curPath.RemoveAt(curPath.Count - 1);
                            if (curPath.Count > 0)
                            {
                                curCase = (DofusCell)curPath.ToArray()[curPath.Count - 1];
                            }
                            else
                            {
                                curCase = map.getCell(start);
                            }
                        }
                        else
                        {
                            curCase = map.getCell(start);
                        }
                    }
                    else
                    {
                        if ((distMax == 0) && (nearestCell == dest))
                        {
                            curPath.Add(map.getCell(dest));
                            break;
                        }
                        if (distMax > GetDistanceBetween(map, nearestCell, dest))
                        {
                            curPath.Add(map.getCell(dest));
                            break;
                        }

                        curCase = map.getCell(nearestCell);
                        closeCells.Add(curCase);
                        curPath.Add(curCase);
                    }
                }

                curCase = map.getCell(start);
                closeCells.Clear();
                closeCells.Add((DofusCell)curPath.ToArray()[0]);

                while ((!stop) && (stepNum++ <= limit))
                {
                    int nearestCell = getNearestCellAround(map, curCase.Id, dest, closeCells,fight);
                    if (nearestCell == -1)
                    {
                        closeCells.Add(curCase);
                        if (curPath2.Count > 0)
                        {
                            curPath2.RemoveAt(curPath2.Count - 1);
                            if (curPath2.Count > 0)
                            {
                                curCase = (DofusCell)curPath2.ToArray()[curPath2.Count - 1];
                            }
                            else
                            {
                                curCase = map.getCell(start);
                            }
                        }
                        else
                        {
                            curCase = map.getCell(start);
                        }
                    }
                    else
                    {
                        if ((distMax == 0) && (nearestCell == dest))
                        {
                            curPath2.Add(map.getCell(dest));
                            break;
                        }
                        if (distMax > GetDistanceBetween(map, nearestCell, dest))
                        {
                            curPath2.Add(map.getCell(dest));
                            break;
                        }

                        curCase = map.getCell(nearestCell);
                        closeCells.Add(curCase);
                        curPath2.Add(curCase);
                    }
                }
                if (((curPath2.Count < curPath.Count) && (curPath2.Count > 0)) || (curPath.Count == 0))
                {
                    curPath = curPath2;
                }
                return curPath;
            }
            catch (Exception e)
            {
            }
            return null;
        }

        public static string GetCellChars(int CellNum)
        {

            string hash = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_";

            int CharCode2 = (CellNum % hash.Length);
            int CharCode1 = (CellNum - CharCode2) / hash.Length;

            return hash[CharCode1].ToString() + hash[CharCode2].ToString();

        }

        public static string GetDirChar(int DirNum)
        {

            string hash = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_";
            if (DirNum >= hash.Length)
                return "";
            return hash[DirNum].ToString();

        }

        public static List<int> getListCaseFromFighter(Fight fight, Fighter fighter)
        {
            try
            {
                List<int> cells = new List<int>();
                int start = fighter.CellId;

                int i = 0;
                int[] curPath;
                if (fighter.MP > 0)
                {
                    curPath = new int[fighter.MP];
                }
                else
                {
                    return null;
                }

                if (curPath.Length == 0)
                {
                    return null;
                }
                do
                {
                    curPath[i] += 1;
                    if ((curPath[i] == 5) && (i != 0))
                    {
                        curPath[i] = 0;
                        i--;
                    }
                    else
                    {
                        int curCell = getCellFromPath(start, curPath);
                        if (!fight.GetCell(curCell).CanWalk())
                        {
                            continue;
                        }
                        if (cells.Contains(curCell))
                        {
                            continue;
                        }
                        cells.Add(curCell);
                        if (i < curPath.Length - 1)
                        {
                            i++;
                        }
                    }
                } while (curPath[0] != 5);

                return triCellList(fight, fighter, cells);
            }
            catch (Exception e)
            {
            }
            return null;
        }

        public static int getCellFromPath(int start, int[] path)
        {
            int cell = start;
            int i = 0;
            while (i < path.Length)
            {
                if (path[i] == 1)
                {
                    cell -= 15;
                }
                if (path[i] == 2)
                {
                    cell -= 14;
                }
                if (path[i] == 3)
                {
                    cell += 15;
                }
                if (path[i] == 4)
                {
                    cell += 14;
                }
                i++;
            }
            return cell;
        }

        public static List<int> triCellList(Fight fight, Fighter fighter, List<int> cells)
        {
            List<int> Fcells = new List<int>();
            List<int> copie = cells;
            int dist = 100;
            int curCell = 0;
            int curIndex = 0;
            while (copie.Count > 0)
            {
                dist = 100;
                foreach (var localIterator in copie)
                {

                    int d = GetDistanceBetween(fight.Map, fighter.CellId, localIterator);
                    if (dist <= d)
                    {
                        continue;
                    }
                    dist = d;
                    curCell = localIterator;
                    curIndex = copie.IndexOf(localIterator);
                }

                Fcells.Add(curCell);
                copie.Remove(curIndex);
            }

            return Fcells;
        }

        public static MovementPath IsValidPath(Fight Fight, Fighter Fighter, int CurrentCell, string EncodedPath)
        {
            var DecodedPath = Pathfinder.DecodePath(Fight.Map, CurrentCell, EncodedPath);
            var FinalPath = new MovementPath();

            var Index = 0;
            int TransitCell = 0;
            do
            {
                TransitCell = DecodedPath.TransitCells[Index];

                var Length = Pathfinder.IsValidLine(Fight, Fighter, FinalPath, TransitCell, DecodedPath.GetDirection(TransitCell), DecodedPath.TransitCells[Index + 1]);
                if (Length == -1)
                    return null;
                else if (Length == -2)
                    break;


                Index++;

            }
            while (TransitCell != DecodedPath.LastStep);

            return FinalPath;
        }

        public static int IsValidLine(Fight Fight, Fighter Fighter, MovementPath Path, int BeginCell, int Direction, int EndCell)
        {
            var Length = -1;
            var ActualCell = BeginCell;

            if (!Pathfinder.InLine(Fight.Map, BeginCell, EndCell))
                return Length;

            Length = (int)GoalDistanceEstimate(Fight.Map, BeginCell, EndCell);

            Path.AddCell(ActualCell, Direction);

            for (int i = 0; i < Length; i++)
            {
                ActualCell = Pathfinder.NextCell(Fight.Map, ActualCell, Direction);

                Path.AddCell(ActualCell, Direction);

                Path.MovementLength++;

                if (Pathfinder.IsStopCell(Fighter.Fight, Fighter.Team, ActualCell, Fighter))
                    return -2;
            }

            return Length;
        }

        public static bool IsStopCell(Fight Fight, FightTeam Team, int CellId, Fighter Fighter)
        {
            // Un piege etc ?
            if (Fight.GetCell(CellId).HasGameObject(FightObjectType.OBJECT_TRAP))
            {
                //Fight.GetCell(CellId).GetObjects<FightTrap>().ForEach(x => x.onTraped(Fighter));
                return true;
            }

            if (GetEnnemyNear(Fight, Team, CellId).Count > 0)
                return true;

            return false;
        }

        public static double GoalDistanceEstimate(Map Map, int BeginCell, int EndCell)
        {
            var loc7 = GetX(Map, BeginCell) - GetX(Map, EndCell);
            var loc8 = GetY(Map, BeginCell) - GetY(Map, EndCell);

            return Math.Sqrt(Math.Pow(loc7, 2) + Math.Pow(loc8, 2));
        }

        public static double GoalDistanceNoSqrt(Map Map, int BeginCell, int EndCell)
        {
            var loc7 = GetX(Map, BeginCell) - GetX(Map, EndCell);
            var loc8 = GetY(Map, BeginCell) - GetY(Map, EndCell);

            return Math.Pow(loc7, 2) + Math.Pow(loc8, 2);
        }

        public static List<Fighter> GetEnnemyNear(Fight Fight, FightTeam Team, int CellId)
        {
            List<Fighter> Ennemies = new List<Fighter>();

            foreach (var Direction in Pathfinder.FIGHT_DIRECTIONS)
            {
                var Ennemy = Fight.HasEnnemyInCell(Pathfinder.NextCell(Fight.Map, CellId, Direction), Team);
                if (Ennemy != null)
                    if (!Ennemy.Dead)
                        Ennemies.Add(Ennemy);
            }

            return Ennemies;
        }

        public static List<int> GetJoinCell(int cell, Map map)
        {
            List<int> cells = new List<int>();
            cells.Add(NextCell(map, cell, 1));
            cells.Add(NextCell(map, cell, 3));
            cells.Add(NextCell(map, cell, 5));
            cells.Add(NextCell(map, cell, 7));
            return cells;
        }


        public static int TryTacle(Fighter Fighter)
        {
            var Ennemies = Pathfinder.GetEnnemyNear(Fighter.Fight, Fighter.Team, Fighter.Cell.Id);

            if (Ennemies.Count == 0 || Ennemies.All(x => x.States.HasState(FighterStateEnum.STATE_ENRACINER)))
                return -1;

            return Pathfinder.TryTacle(Fighter, Ennemies);
        }

        private static int TryTacle(Fighter Fighter, IEnumerable<Fighter> NearestEnnemies)
        {
            var FighterAgility = Fighter.Stats.GetTotal(EffectEnum.AddAgilite);
            int EnnemiesAgility = 0;

            foreach (var Ennemy in NearestEnnemies)
                if (!Ennemy.States.HasState(FighterStateEnum.STATE_ENRACINER))
                    EnnemiesAgility += Ennemy.Stats.GetTotal(EffectEnum.AddAgilite);

            var A = FighterAgility + 25;
            var B = FighterAgility + EnnemiesAgility + 50;
            var Chance = (int)((long)(300 * A / B) - 100);
            var Rand = Pathfinder.PATHFIND_RANDOM.Next(0, 99);

            return Rand > Chance ? Rand : -1;
        }

        public static int GetDistanceBetween(Map Map, int id1, int id2)
        {
            if (id1 == id2) return 0;
            if (Map == null) return 0;
            int diffX = Math.Abs(GetCellXCoord(id1, Map.Width) - GetCellXCoord(id2, Map.Width));
            int diffY = Math.Abs(GetCellYCoord(id1, Map.Width) - GetCellYCoord(id2, Map.Width));
            return (diffX + diffY);
        }

        public static int GetCellXCoord(int cellid, int width)
        {
            int w = width;
            return ((cellid - (w - 1) * GetCellYCoord(cellid, width)) / w);
        }

        public static int GetCellYCoord(int cellid, int width)
        {
            int w = width;
            int loc5 = (int)(cellid / ((w * 2) - 1));
            int loc6 = cellid - loc5 * ((w * 2) - 1);
            int loc7 = loc6 % w;
            return (loc5 - loc7);
        }


        public static int GoalDistance(Map Map, int BeginCell, int EndCell)
        {
            var Xbegin = (int)GetX(Map, BeginCell);
            var Xend = (int)GetX(Map, EndCell);
            var Ybegin = (int)GetY(Map, BeginCell);
            var Yend = (int)GetY(Map, EndCell);
            return Math.Abs(Xbegin - Xend) + Math.Abs(Ybegin - Yend);
        }

        public static int OppositeDirection(int Direction)
        {
            return (Direction >= 4 ? Direction - 4 : Direction + 4);
        }


        public static bool InLine(Map Map, int BeginCell, int EndCell)
        {
            try
            {
                if (BeginCell == EndCell) return true;
                if (GetX(Map, BeginCell) == GetX(Map, EndCell) && GetY(Map, BeginCell) == GetY(Map, EndCell))
                {
                    return false;
                }
                return GetX(Map, BeginCell) == GetX(Map, EndCell) || GetY(Map, BeginCell) == GetY(Map, EndCell);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static int NextCell(Map Map, int Cell, int Direction)
        {
            switch (Direction)
            {
                case 0:
                    return Cell + 1;
                case 1:
                    return Cell + Map.Width;
                case 2:
                    return Cell + (Map.Width * 2) - 1;
                case 3:
                    return Cell + Map.Width - 1;
                case 4:
                    return Cell - 1;
                case 5:
                    return Cell - Map.Width;
                case 6:
                    return Cell - (Map.Width * 2) - 1;
                case 7:
                    return Cell - Map.Width + 1;
                default:
                    return -1;
            }
        }

        public static int IsValidLine(Map Map, int BeginCell, int Direction, int EndCell)
        {
            var Length = -1;
            var ActualCell = BeginCell;

            if (Pathfinder.InLine(Map, BeginCell, EndCell))
                Length = (int)GoalDistanceEstimate(Map, BeginCell, EndCell);
            else
                Length = (int)(GoalDistanceEstimate(Map, BeginCell, EndCell) / 1.4);

            for (int i = 0; i < Length; i++)
            {
                ActualCell = Pathfinder.NextCell(Map, ActualCell, Direction);
                if (!Map.IsCellWalkable(ActualCell)) ;
                //return -1;

                //return -1;
            }

            return Length;
        }

        public static void GenerateGrid(int Width, int CellsCount)
        {
            var Grid = new Dictionary<int, Point>(CellsCount);

            for (int i = 0; i < CellsCount; i++)
            {
                Grid.Add(i, new Point(i, _GetX(Width, i), _GetY(Width, i)));
            }

            myCellToPointGrid.Add(CellsCount, Grid);
        }

        private static double _GetX(int Width, int Cell)
        {
            double loc5 = Math.Floor((double)(Cell / (Width * 2 - 1)));
            double loc6 = Cell - loc5 * (Width * 2 - 1);
            double loc7 = loc6 % Width;

            return (Cell - (Width - 1) * (loc5 - loc7)) / Width;
        }

        private static double _GetY(int Width, int Cell)
        {
            double loc5 = Math.Floor((double)(Cell / (Width * 2 - 1)));
            double loc6 = Cell - loc5 * (Width * 2 - 1);
            double loc7 = loc6 % Width;

            return loc5 - loc7;
        }

        public static MovementPath DecodePath(Map Map, int CurrentCell, string Path)
        {
            MovementPath MovementPath = new MovementPath();

            MovementPath.AddCell(CurrentCell, GetDirection(Map, CurrentCell, CellHelper.CellCharCodeToId(Path.Substring(1, 2))));

            for (int i = 0; i < Path.Length; i += 3)
            {
                int curCell = CellHelper.CellCharCodeToId(Path.Substring(i + 1, 2));
                int curDir = CellHelper.HASH.IndexOf(Path[i]);

                MovementPath.AddCell(curCell, curDir);
            }

            return MovementPath;
        }

        public static String EncodePath(Map map, int CurrentCell, MovementPath DecodePath)
        {
            return DecodePath.ToString();
        }

        public static int GetDirection(Map Map, int BeginCell, int EndCell)
        {
            var ListChange = new int[] { 1, Map.Width, Map.Width * 2 - 1, Map.Width - 1, -1, -Map.Width, -Map.Width * 2 + 1, -(Map.Width - 1) };
            var Result = EndCell - BeginCell;

            for (int i = 7; i > -1; i--)
                if (Result == ListChange[i])
                    return i;

            var ResultX = GetX(Map, EndCell) - GetX(Map, BeginCell);
            var ResultY = GetY(Map, EndCell) - GetY(Map, BeginCell);

            if (ResultX == 0)
                if (ResultY > 0)
                    return 3;
                else
                    return 7;
            else if (ResultX > 0)
                return 1;
            else
                return 5;
        }

        public static double GetX(Map Map, int Cell)
        {
            if (!myCellToPointGrid.ContainsKey(Map.CellsCount))
                Pathfinder.GenerateGrid(Map.Width, Map.CellsCount);

            Point p = new Point();

            if (myCellToPointGrid[Map.CellsCount].TryGetValue(Cell, out p))
                return p.X;

            return -1000;
        }

        public static double GetY(Map Map, int Cell)
        {
            if (!myCellToPointGrid.ContainsKey(Map.CellsCount))
                Pathfinder.GenerateGrid(Map.Width, Map.CellsCount);

            Point p = new Point();

            if (myCellToPointGrid[Map.CellsCount].TryGetValue(Cell, out p))
                return p.Y;

            return -1000;
        }

        public static Point GetCoordinates(Map Map, int Cell)
        {
            if (!myCellToPointGrid.ContainsKey(Map.CellsCount))
                Pathfinder.GenerateGrid(Map.Width, Map.CellsCount);
            Point p = new Point();
            myCellToPointGrid[Map.CellsCount].TryGetValue(Cell, out p);
            return p;
        }

        public static char GetDirectionChar(int Direction)
        {
            return CellHelper.HASH[Direction];
        }

        public static bool CheckCellView(Map Map, Fight fight, double x, double y, bool boolean, Point p1, Point p2, double zDiff, double d)
        {
            var CellId = GetCellNumber(Map, x, y);
            FightCell cell = fight.GetCell(CellId);
            if(cell==null){
                return true;
            }
            double YZMaxDiff = Math.Max(Math.Abs(p1.Y - y), Math.Abs(p1.X - x));
            var Gradient = YZMaxDiff / d * zDiff + p1.Z;
            var CellHeight = GetCellHeight(Map, CellId);
            var IsFull = cell.HasUnWalkableFighter() || (YZMaxDiff == 0 || (boolean || p2.X == x && p2.Y == y)) ? (false) : (true);
            bool ok = cell.LineOfSight() && (CellHeight <= Gradient && !cell.HasUnWalkableFighter());
            if (!ok)
            {
                //fight.SendToFight(new FightShowCell(fight.Fighters[0].ActorId, CellId));
                return boolean;
            }
            else
            {
                return true;
            }
        }

        public static int GetCellNumber(Map Map, double x, double y)
        {
            return (int)(Map.Width * x + (Map.Width - 1) * y);
        }

        public static bool CheckView(Map Map, Fight fight, int Cell1, int Cell2)
        {
            Point Cell1_Point = new Point(GetCoordinates(Map, Cell1));
            Point Cell2_Point = new Point(GetCoordinates(Map, Cell2));

            FightCell cell1 = fight.GetCell(Cell1);
            FightCell cell2 = fight.GetCell(Cell2);

            double Cell1_ZSpriteOnID = !cell1.IsWalkable() ? (1.5) : (0);
            Cell1_ZSpriteOnID += cell1.HasUnWalkableFighter() ? (1.5) : (0);
            double Cell2_ZSpriteOnID = !cell2.IsWalkable() ? (1.5) : (0);
            Cell2_ZSpriteOnID += cell2.HasUnWalkableFighter() ? (1.5) : (0);

            Cell1_Point.Z = GetCellHeight(Map, Cell1) + Cell1_ZSpriteOnID;
            Cell2_Point.Z = GetCellHeight(Map, Cell2) + Cell2_ZSpriteOnID;

            double ZDiff = Cell2_Point.Z - Cell1_Point.Z;
            double YZMaxDiff = Math.Max(Math.Abs(Cell1_Point.Y - Cell2_Point.Y), Math.Abs(Cell1_Point.X - Cell2_Point.X));

            double CoeffDirector = (Cell1_Point.Y - Cell2_Point.Y) / (Cell1_Point.X - Cell2_Point.X);
            double YTranslation_Cell1 = Cell1_Point.Y - CoeffDirector * Cell1_Point.X;

            int XReport = Cell2_Point.X - Cell1_Point.X < 0 ? (-1) : (1);
            int YReport = Cell2_Point.Y - Cell1_Point.Y < 0 ? (-1) : (1);

            double Cell1_X = Cell1_Point.X;
            double Cell1_Y = Cell1_Point.Y;

            double XTranslation_Cell2 = Cell2_Point.X * XReport;
            double YTranslation_Cell2 = Cell2_Point.Y * YReport;
            double XTranslation_Cell1 = Cell1_Point.X + 0.5 * XReport;

            var _loc26 = Cell1_Y;

            while ((XTranslation_Cell1 += XReport) * XReport <= XTranslation_Cell2)
            {
                var _loc25 = CoeffDirector * XTranslation_Cell1 + YTranslation_Cell1;

                double _loc21 = 0;
                double _loc22 = 0;
                if (YReport > 0)
                {
                    _loc21 = Math.Round(_loc25);
                    _loc22 = Math.Ceiling(_loc25 - 0.5);
                }
                else
                {
                    _loc21 = Math.Ceiling(_loc25 - 0.5);
                    _loc22 = Math.Round(_loc25);
                } // end else if

                _loc26 = Cell1_Y;

                while ((_loc26 += YReport) * YReport <= _loc22 * YReport)
                {
                    if (!CheckCellView(Map, fight, XTranslation_Cell1 - XReport / 2, _loc26, false, Cell1_Point, Cell2_Point, ZDiff, YZMaxDiff))
                    {
                        return false;
                    }
                }
                
                Cell1_Y = _loc21;
            }

            _loc26 = Cell1_Y;

            while (( _loc26 += YReport) * YReport <= Cell2_Point.Y * YReport)
            {
                if (!CheckCellView(Map, fight, XTranslation_Cell1 - 0.5 * XReport, _loc26, false, Cell1_Point, Cell2_Point, ZDiff, YZMaxDiff))
                {
                    return false;
                }
            }

            if (!CheckCellView(Map, fight, XTranslation_Cell1 - 0.5 * XReport, _loc26 - YReport, true, Cell1_Point, Cell2_Point, ZDiff, YZMaxDiff))
            {
                return false;
            }

            return true;
        }

        public static double GetCellHeight(Map Map, int CellId)
        {
            DofusCell cell = Map.getCell(CellId);
            if (cell == null)
            {
                return 0;
            }
            else
            {
                return (cell.groundSlope == 1 ? (0) : (0.5))+(cell.groundLevel - 7);
            }
        }

        public static int GetDirection(char Direction)
        {
            return CellHelper.HASH.IndexOf(Direction);
        }

        public static double GetPathTime(int Len)
        {
            return ((Len >= 6 ? Pathfinder.RUN_SPEED : Pathfinder.WALK_SPEED) * 1000 * Len);
        }

        public static int ForViewOrientation(int Orientation)
        {
            switch (Orientation)
            {
                case 0:
                    return 1;
                case 2:
                    return 3;
                case 4:
                    return 5;
                case 6:
                    return 7;
            }
            return 1;
        }

        public static int GetAggroDistanceByLevel(int lvl)
        {
            int aggro = 0;
            aggro = (int)(lvl / 50);
            if (lvl > 500)
            {
                aggro = 3;
            }
            return aggro;
        }

        public static char getDirBetweenTwoCase(int cell1ID, int cell2ID, Map map, Boolean Combat)
        {
            char[] dirs = { 'b', 'd', 'f', 'h' };

            if (!Combat)
            {
                char[] combatDirs = { 'a', 'b', 'c', 'd' };
                dirs.Concat(combatDirs);
            }
            foreach (char c in dirs)
            {
                int cell = cell1ID;
                for (int i = 0; i <= 64; i++)
                {
                    if (DirToCellID(cell, c, map, Combat) == cell2ID)
                    {
                        return c;
                    }
                    cell = DirToCellID(cell, c, map, Combat);
                }
            }
            return (char)0;
        }

        public static int DirToCellID(int CaseID, char Direction, Map map, Boolean Combat)
        {
            switch (Direction)
            {
                case 'a':
                    return Combat ? -1 : CaseID + 1;
                case 'b':
                    return CaseID + map.Width;
                case 'c':
                    return Combat ? -1 : CaseID + (map.Width * 2 - 1);
                case 'd':
                    return CaseID + (map.Width - 1);
                case 'e':
                    return Combat ? -1 : CaseID - 1;
                case 'f':
                    return CaseID - map.Width;
                case 'g':
                    return Combat ? -1 : CaseID - (map.Width * 2 - 1);
                case 'h':
                    return CaseID - map.Width + 1;
            }
            return -1;
        }

        public static List<DofusCell> getCellListFromAreaString(Map map, int cellID, int castCellID, String zoneStr, int PONum, Boolean isCC)
        {
            List<DofusCell> cases = new List<DofusCell>();
            int c = PONum;
            if (map.getCell(cellID) == null)
            {
                return cases;
            }
            cases.Add(map.getCell(cellID));

            if (zoneStr.Length < c + 1)
            {
                return cases;
            }
            int taille = StringHelper.HashToInt(zoneStr[c + 1]);
            switch (zoneStr[c])
            {
                case 'C'://Cercle
                    for (int a = 0; a < taille; a++)
                    {
                        char[] dirs = { 'b', 'd', 'f', 'h' };
                        List<DofusCell> cases2 = new List<DofusCell>();//on évite les modifications concurrentes
                        cases2.AddRange(cases);
                        foreach (DofusCell aCell in cases2)
                        {
                            foreach (char d in dirs)
                            {
                                DofusCell cell = map.getCell(DirToCellID(aCell.Id, d, map, true));
                                if (cell == null)
                                {
                                    continue;
                                }
                                if (!cases.Contains(cell))
                                {
                                    cases.Add(cell);
                                }
                            }
                        }
                    }
                    break;

                case 'X'://Croix
                    char[] dirs2 = { 'b', 'd', 'f', 'h' };
                    foreach (char d in dirs2)
                    {
                        int cID = cellID;
                        for (int a = 0; a < taille; a++)
                        {
                            DofusCell cell = map.getCell(DirToCellID(cID, d, map, true));
                            cID = DirToCellID(cID, d, map, true);
                            if (cell == null)
                            {
                                continue;
                            }
                            cases.Add(cell);
                        }
                    }
                    break;

                case 'L'://Ligne
                    char dir = getDirBetweenTwoCase(castCellID, cellID, map, true);
                    for (int a = 0; a < taille; a++)
                    {
                        DofusCell cell = map.getCell(DirToCellID(cellID, dir, map, true));
                        if (cell != null)
                        {
                            cases.Add(cell);
                        }
                        cellID = DirToCellID(cellID, dir, map, true);
                    }
                    break;

                case 'P'://Player?

                    break;

                default:
                    Logger.Error("[FIXME]Type de portée non reconnue: " + zoneStr[0]);
                    break;
            }
            return cases;
        }

        public static int GoalDistanceScore(Map Map, int BeginCell, int EndCell)
        {
            var Xbegin = (int)GetX(Map, BeginCell);
            var Xend = (int)GetX(Map, EndCell);
            var Ybegin = (int)GetY(Map, BeginCell);
            var Yend = (int)GetY(Map, EndCell);
            var XDiff = Math.Abs(Xbegin - Xend);
            var YDiff = Math.Abs(Ybegin - Yend);

            return XDiff + YDiff;
        }
    }
}
