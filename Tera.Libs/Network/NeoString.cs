using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tera.Libs.Network
{
    public class NeoString
    {
        private int[] pwd_sbox = null;
        private int[] pwd_mykey = null;

        private int[] CopySbox()
        {
             int[] sbox = new int[256];
             System.Array.Copy(pwd_sbox, sbox, 256);
             return sbox;
        }
        private int[] CopyMyKey()
        {
            int[] mykey = new int[256];
            System.Array.Copy(pwd_mykey, mykey, 256);
            return mykey;
        }

        private void initialize(char[] pwd)
        {
            pwd_sbox = new int[256];
            pwd_mykey = new int[256];
            int b = 0;
            int tempSwap;
            int intLength = pwd.Length;
            for (int a = 0; a <= 255; a++)
            {
                pwd_mykey[a] = pwd[a % intLength];
                pwd_sbox[a] = a;
            }
            for (int a = 0; a <= 255; a++)
            {
                b = toUnsigned(b + pwd_sbox[a] + pwd_mykey[a] % 256);
                tempSwap = pwd_sbox[a];
                pwd_sbox[a] = pwd_sbox[b];
                pwd_sbox[b] = tempSwap;
            }
        }

        public NeoString(String key)
        {
            initialize(key.ToCharArray());
        }

        private int toUnsigned(int i)
        {
            while (i < 0)
            {
                i += 255;
            }
            while (i > 255)
            {
                i -= 255;
            }
            return i;
        }

        private char[] calculate(char[] plaintxt)
        {
            int[] sbox = CopySbox();
            int[] mykey = CopyMyKey();

            int i = 0, j = 0;
            List<char> cipher = new List<char>();
            int k = 0, temp, cipherby;
            for (int a = 0; a < plaintxt.Length; a++)
            {
                i = toUnsigned(i + 1 % 256);
                j = toUnsigned(j + sbox[i] % 256);
                temp = sbox[i];
                int SJ = 0;
                SJ = sbox[j];
                sbox[i] = SJ;
                sbox[j] = temp;
                int SI = 0;
                SI = sbox[i];
                SJ = sbox[j];
                int idx = toUnsigned(SI + SJ % 256);
                k = sbox[idx];
                cipherby = plaintxt[a] ^ k;
                cipher.Add(((char)(cipherby)));
            }
            char[] toReturn = new char[cipher.Count];
            for (int r = 0; r < cipher.Count; r++)
            {
                toReturn[r] = cipher.ToArray()[r];
            }
            return toReturn;
        }

        public String encrypt(String src)
        {
            char[] result = calculate(strToChars(src));
            return charsToHex(result);
        }

        public String decrypt(String src)
        {
            char[] result = calculate(hexToCharsAS(src));
            return charsToStr(result);
        }

        private static string[] hexes = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "a", "b", "c", "d", "e", "f" };
        public static String charsToHex(char[] chars)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < chars.Length; i++)
            {
                result.Append(hexes[chars[i] >> 4]).Append(hexes[chars[i] & 0xf]);
            }
            return result.ToString();
        }

        private static String charsToStr(char[] chars)
        {
            return new String(chars);
        }

        private static char[] strToChars(String str)
        {
            return str.ToCharArray();
        }

        private char[] hexToCharsAS(String hex)
        {
            ArrayList codes = new ArrayList();
            for (int i = "0x".Equals(hex.Substring(0, 2)) ? 2 : 0; i < hex.Length; i += 2)
            {
                try
                {
                    codes.Add((char)Convert.ToInt32(hex.Substring(i, 2), 16));
                }
                catch (Exception e)
                {
                    break;
                }
            }
            char[] toReturn = new char[codes.Count];
            for (int i = 0; i < codes.Count; i++)
            {
                toReturn[i] = (char)codes[i];
            }
            codes.Clear();
            return toReturn;
        }

        private static int[] charsToLongs(char[] chars)
        {
            int ceil = (int)Math.Round(Math.Ceiling((double)chars.Length / 4));
            if (ceil == 0)
            {
                ceil = 1;
            }
            int[] temp = new int[ceil];
            for (int i = 0; i < temp.Length; i++)
            {
                int i1 = 0;
                try
                {
                    i1 = chars[i * 4];
                }
                catch (Exception e)
                {
                }
                int i2 = 0;
                try
                {
                    i2 = (chars[i * 4 + 1] << 8);
                }
                catch (Exception e)
                {
                }
                int i3 = 0;
                try
                {
                    i3 = (chars[i * 4 + 2] << 16);
                }
                catch (Exception e)
                {
                }
                int i4 = 0;
                try
                {
                    i4 = (chars[i * 4 + 3] << 24);
                }
                catch (Exception e)
                {
                }
                temp[i] = i1 + i2 + i3 + i4;
            }
            return temp;
        }

    }
}
