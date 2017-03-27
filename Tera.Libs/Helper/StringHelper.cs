using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tera.Libs.Helper
{
    static public class StringHelper
    {
        public static Random Randomizer = new Random();
        public static Random Rand = new Random(Environment.TickCount);
        public static string[] LettersPairs = { "lo", "la", "li", "wo", "wi", "ka", "ko", "ki", "po",
                                                  "pi", "pa", "aw", "al", "na", "ni", "ny", "no", "ba", "bi",
                                                  "ra", "ri", "ze", "za", "da", "zel", "wo", "-" };

        public static char[] HASH = {'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's',
             't', 'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U',
             'V', 'W', 'X', 'Y', 'Z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '-', '_'};


        public static int HashToInt(char c)
        {
            for (int a = 0; a < HASH.Length; a++)
            {
                if (HASH[a] == c)
                {
                    return a;
                }
            }
            return -1;
        }

        public static char IntToHash(int c)
        {
            return HASH[c];
        }

        public static string RandomString(int lenght)
        {
            string str = string.Empty;
            for (int i = 1; i <= lenght; i++)
            {
                int randomInt = Randomizer.Next(0, HASH.Length);
                str += HASH[randomInt];
            }
            return str;
        }

        public static String EncodeBase36(int intNumber)
        {
            string text = "";
            int num = 36;
            do
            {
                int num2 = intNumber % num;
                bool flag = num2 > 9;
                if (flag)
                {
                    text = (char)(num2 + 87) + text;
                }
                else
                {
                    text = num2.ToString() + text;
                }
                intNumber = (int)Math.Round(Math.Floor((double)intNumber / (double)num));
            }
            while (intNumber != 0);
            return text;
        }

        public static int RandomNumber(int min, int max)
        {
            return Rand.Next(min, max);
        }

        public static string GetRandomName()
        {
            string Name = "";
            for (int i = 0; i <= RandomNumber(2, 4); i++)
            {
                Name += LettersPairs[RandomNumber(0, LettersPairs.Length - 1)];
            }
            Name = Name.ToCharArray()[0].ToString().ToUpper() + Name.Substring(1);
            return Name;
        }

        public static Boolean isValidName(String name)
        {
            Boolean isValidName = true;
            int tiretCount = 0;
            char exLetterA = ' ';
            char exLetterB = ' ';
            foreach (char curLetter in name.ToLower().ToCharArray())
            {
                if (!((curLetter >= 'a' && curLetter <= 'z') || curLetter == '-'))
                {
                    isValidName = false;
                    break;
                }
                if (curLetter == exLetterA && curLetter == exLetterB)
                {
                    isValidName = false;
                    break;
                }
                if ((curLetter >= 'a' && curLetter <= 'z'))
                {
                    exLetterA = exLetterB;
                    exLetterB = curLetter;
                }
                if (curLetter == '-')
                {
                    if (tiretCount >= 1)
                    {
                        isValidName = false;
                        break;
                    }
                    else
                    {
                        tiretCount++;
                    }
                }
            }
            return isValidName;
        }
    }
}
