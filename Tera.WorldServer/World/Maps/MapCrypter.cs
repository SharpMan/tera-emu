using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.WorldServer.Database.Models;

namespace Tera.WorldServer.World.Maps
{
    public class MapCrypter
    {
        const String HASH_CELL = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        public static List<char> HASH = new List<char>() {'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's',
                't', 'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U',
                'V', 'W', 'X', 'Y', 'Z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '-', '_'};

        public static string CellIdToCharCode(int cellID)
        {

            int char1 = cellID / 64,
                char2 = cellID % 64;
            return string.Concat(HASH[char1].ToString(), HASH[char2].ToString());
        }

        public static int CellCharCodeToId(String cellCode)
        {
            char char1 = cellCode[0], char2 = cellCode[1];
            int code1 = 0, code2 = 0, a = 0;
            while (a < HASH.Count)
            {
                if (HASH[a] == char1)
                {
                    code1 = a * 64;
                }
                if (HASH[a] == char2)
                {
                    code2 = a;
                }
                a++;
            }
            return (code1 + code2);
        }


        public static Dictionary<int, DofusCell> DecompileMapData(Map map, String dData)
        {
            Dictionary<int, DofusCell> cells = new Dictionary<int, DofusCell>();
            for (int f = 0; f < dData.Length; f += 10)
            {
                String CellData = dData.Substring(f, 10);
                int[] CellInfo = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                for (int i = 0; i < CellData.Length; i++)
                {
                    CellInfo[i] = HASH_CELL.IndexOf(CellData[i]);
                }
                int Type = (CellInfo[2] & 56) >> 3;
                Boolean IsSightBlocker = (CellInfo[0] & 1) != 0;
                int layerObject2 = ((CellInfo[0] & 2) << 12) + ((CellInfo[7] & 1) << 12) + (CellInfo[8] << 6) + CellInfo[9];
                Boolean layerObject2Interactive = ((CellInfo[7] & 2) >> 1) != 0;
                int obj = (layerObject2Interactive ? layerObject2 : -1);
                Boolean _walkable = ((Type != 0) && ((CellInfo[0] & 32) >> 5) != 0);
                int GroundSlope = (CellInfo[4] & 60) >> 2;
                int GroundLevel = CellInfo[1] & 15;

                var cell = new DofusCell()
                {
                    Map = map.Id,
                    Id = f/10,
                    Walkable = _walkable,
                    LoS = IsSightBlocker,
                    groundLevel = GroundLevel,
                    groundSlope = GroundSlope
                };
                if (obj != -1)
                {
                    cell.Object = new IObject(map, cell, obj);
                }
                cells.Add(f/10, cell);
            }
            return cells;
        }
    }
}
