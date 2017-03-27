using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.Libs.Enumerations;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.World.Fights;

namespace Tera.WorldServer.World.Maps
{
    public static class CellZone
    {
        public static List<int> GetAdjacentCells(Map Map, int Cell)
        {
            var Cells = new List<int>();
            for (int i = 1; i < 8; i += 2)
                Cells.Add(Pathfinder.NextCell(Map, Cell, i));

            return Cells;
        }
        /**
         * 
         * dirs.Add('b');
            dirs.Add('d');
            dirs.Add('f');
            dirs.Add('h');
            if (!Combat)
            {
                dirs.Add('a');
                dirs.Add('c');
                dirs.Add('e');
                dirs.Add('g');
            }
         * */
        private static string Dirs = "bdfhaceg";
        public static bool CellsInSameLineFight(Map Map, int BeginCell, int EndCell, int BaseDirection)
        {
            int BaseDirectionOpp = Pathfinder.OppositeDirection(BaseDirection);
            int dir = Pathfinder.GetDirection(Map, BeginCell, EndCell);
            if (dir > 3)
            {
                Logger.Info("Dir > 3!!");
                return false;
            }
            return dir == BaseDirection || dir == BaseDirectionOpp;
        }

        public static List<int> GetLineCells(Map Map, int Cell, int Direction, int Length)
        {
            var Cells = new List<int>();
            var LastCell = Cell;
            for (int i = 0; i < Length; i++)
            {
                Cells.Add(Pathfinder.NextCell(Map, LastCell, Direction));
                LastCell = Cells[i];
            }
            return Cells;
        }

        public static List<int> GetCircleCells(Map Map, int CurrentCell, int Radius)
        {
            var Cells = new List<int>() { CurrentCell };
            for (int i = 0; i < Radius; i++)
            {
                var Copy = Cells.ToArray();
                foreach (var Cell in Copy)
                    Cells.AddRange(from Item in GetAdjacentCells(Map, Cell) where !Cells.Contains(Item) select Item);
            }

            return Cells;
        }

        public static List<int> GetCrossCells(Map Map, int CurrentCell, int Radius)
        {
            var Cells = new List<int>();
            foreach (var Cell in GetCircleCells(Map, CurrentCell, Radius))
                if (Pathfinder.InLine(Map, CurrentCell, Cell))
                    Cells.Add(Cell);

            return Cells;
        }

        public static List<int> GetTLineCells(Map Map, int Cell, int Direction, int Length)
        {
            var LineDirection = Direction <= 5 ? Direction + 2 : Direction - 6;
            var Cells = new List<int>();

            Cells.AddRange(GetLineCells(Map, Cell, LineDirection, Length));
            Cells.AddRange(GetLineCells(Map, Cell, Pathfinder.OppositeDirection(LineDirection), Length));

            return Cells;
        }

        public static List<int> GetWeaponCells(int WeaponType, Map map, FightCell Cell, int CasterCell)
        {
            List<int> Cells = new List<int>();
            char c = getDirBetweenTwoCase(CasterCell, Cell.Id, map, true);
            if (c == (char)0)
            {
                if (Cell.HasGameObject(FightObjectType.OBJECT_FIGHTER) | Cell.HasGameObject(FightObjectType.OBJECT_STATIC))
                {
                    Cells.Add(Cell.Id);
                    return Cells;
                }
            }
            switch((ItemTypeEnum)WeaponType)
            {
                case ItemTypeEnum.ITEM_TYPE_MARTEAU:
                    Cells.Add(getFighter2CellBefore(CasterCell, c, map));
                    Cells.Add(get1StFighterOnCellFromDirection(map, CasterCell, (char)(c - 1)));
                    Cells.Add(get1StFighterOnCellFromDirection(map, CasterCell, (char)(c + 1)));
                    Cells.Add(Cell.Id);
                    break;
                case ItemTypeEnum.ITEM_TYPE_BATON:
                    Cells.Add(get1StFighterOnCellFromDirection(map, CasterCell, (char)(c - 1)));
                    Cells.Add(get1StFighterOnCellFromDirection(map, CasterCell, (char)(c + 1)));
                    Cells.Add(Cell.Id);
                    break;
                case ItemTypeEnum.ITEM_TYPE_PIOCHE:
                case ItemTypeEnum.ITEM_TYPE_EPEE:
                case ItemTypeEnum.ITEM_TYPE_FAUX:
                case ItemTypeEnum.ITEM_TYPE_DAGUES:
                case ItemTypeEnum.ITEM_TYPE_BAGUETTE:
                case ItemTypeEnum.ITEM_TYPE_PELLE:
                case ItemTypeEnum.ITEM_TYPE_ARC:
                case ItemTypeEnum.ITEM_TYPE_HACHE:
                    Cells.Add(Cell.Id);
                    break;
            }
            return Cells;
        }

        private static int getFighter2CellBefore(int CellID, char c, Map map)
        {
            return GetCaseIDFromDirrection(GetCaseIDFromDirrection(CellID, c, map, false), c, map, false);
        }

        private static int get1StFighterOnCellFromDirection(Map map, int id, char c)
        {
            if (c == (char)('a' - 1))
                c = 'h';
            if (c == (char)('h' + 1))
                c = 'a';
            return map.getCell(GetCaseIDFromDirrection(id, c, map, false)).Id;
        }

        public static char getDirBetweenTwoCase(int cell1ID, int cell2ID, Map map, Boolean Combat)
        {
            // ne permet d'avoir que les directions uniques (pas de composition de direction)
            List<char> dirs = new List<char>();
            dirs.Add('b');
            dirs.Add('d');
            dirs.Add('f');
            dirs.Add('h');
            if (!Combat)
            {
                dirs.Add('a');
                dirs.Add('c');
                dirs.Add('e');
                dirs.Add('g');
            }
            foreach (char c in dirs) // pour chaque direction
            {
                int cell = cell1ID; // on considre la case de dt
                for (int i = 0; i <= 64; i++)
                {
                    if (GetCaseIDFromDirrection(cell, c, map, Combat) == cell2ID) // si pour cette direction la prochaine case est l'arrive
                        return c; // on renvoie la direction
                    cell = GetCaseIDFromDirrection(cell, c, map, Combat); // on continue dans cette direction
                }
            }
            return (char)0;
        }

        public static int GetCaseIDFromDirrection(int CaseID, char Direction, Map map, Boolean Combat)
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

        public static List<int> GetCells(Map Map, int Cell, int CurrentCell, string Range)
        {
            switch (Range[0])
            {
                case 'C':
                    return GetCircleCells(Map, Cell, MapCrypter.HASH.IndexOf(Range[1]));

                case 'X':
                    return GetCrossCells(Map, Cell, MapCrypter.HASH.IndexOf(Range[1]));

                case 'T':
                    var Cells1 = new List<int> { Cell };
                    Cells1.AddRange(GetTLineCells(Map, Cell, Pathfinder.GetDirection(Map, CurrentCell, Cell), MapCrypter.HASH.IndexOf(Range[1])));
                    return Cells1;

                case 'L':
                    var Cells2 = new List<int> { Cell };
                    Cells2.AddRange(GetLineCells(Map, Cell, Pathfinder.GetDirection(Map, CurrentCell, Cell), MapCrypter.HASH.IndexOf(Range[1])));
                    return Cells2;
            }

            return new List<int>() { Cell };
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
            int taille = MapCrypter.HASH.IndexOf(zoneStr[c + 1]);
            switch (zoneStr[c])
            {
                case 'C'://Cercle
                    for (int a = 0; a < taille; a++)
                    {
                        char[] dirs = { 'b', 'd', 'f', 'h' };
                        List<DofusCell> cases2 = new List<DofusCell>();
                        cases.ForEach(x => cases2.Add(x));
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
                    char[] dirsx = { 'b', 'd', 'f', 'h' };
                    foreach (char d in dirsx)
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
    }
}
