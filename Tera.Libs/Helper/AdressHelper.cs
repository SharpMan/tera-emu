using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Tera.Libs.Helper
{
    public class AdressHelper
    {
        public static String CryptIP(String IP)
        {
            String[] Splitted = Regex.Split(IP, "\\.");
            StringBuilder Encrypted = new StringBuilder("");
            int Count = 0;
            for (int i = 0; i < 50; i++)
            {
                for (int o = 0; o < 50; o++)
                {
                    if (((i & 15) << 4 | o & 15) == Convert.ToInt32(Splitted[Count]))
                    {
                        Char A = (char)(i + 48);
                        Char B = (char)(o + 48);
                        Encrypted.Append(A.ToString()).Append(B.ToString());
                        i = 0;
                        o = 0;
                        Count++;
                        if (Count == 4)
                            return Encrypted.ToString();
                    }
                }
            }
            return "DD";
        }

        public static String CryptPort(int config_game_port)
        {
            char[] HASH = {'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's',
	            't', 'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U',
	            'V', 'W', 'X', 'Y', 'Z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '-', '_'};
            int P = config_game_port;
            String nbr64 = "";
            for (int a = 2; a >= 0; a--)
            {
                nbr64 += HASH[(int)(P / (Math.Pow(64, a)))];
                P = (int)(P % (int)(Math.Pow(64, a)));
            }
            return nbr64;
        }
    }
}
