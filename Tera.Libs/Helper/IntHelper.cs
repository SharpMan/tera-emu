using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tera.Libs.Helper
{
    public static class IntHelper
    {
        public static int MIN_RADIX = 2;
        public static int MAX_RADIX = 36;
        private static int FAST_PATH_MAX = 255;
        static char[] digits = {
                                   '0' , '1' , '2' , '3' , '4' , '5' ,
                                   '6' , '7' , '8' , '9' , 'a' , 'b' ,
	                               'c' , 'd' , 'e' , 'f' , 'g' , 'h' ,
	                               'i' , 'j' , 'k' , 'l' , 'm' , 'n' ,
	                               'o' , 'p' , 'q' , 'r' , 's' , 't' ,
	                               'u' , 'v' , 'w' , 'x' , 'y' , 'z'
        };

        /**
          * The minimum value of a Unicode code point.
          * 
          * @since 1.5
        */
        public static int MIN_CODE_POINT = 0x000000;

        /**
         * The maximum value of a Unicode code point.
         *
         * @since 1.5
        */
        public static int MAX_CODE_POINT = 0x10ffff;


        public static String toString(int i, int radix)
        {

            if (radix < MIN_RADIX || radix > MAX_RADIX)
                radix = 10;

            /* Use the faster version */
            if (radix == 10)
            {
                // return ToString(i);
                return i.ToString();
            }

            char[] buf = new char[33];
            Boolean negative = (i < 0);
            int charPos = 32;

            if (!negative)
            {
                i = -i;
            }

            while (i <= -radix)
            {
                buf[charPos--] = digits[-(i % radix)];
                i = i / radix;
            }
            buf[charPos] = digits[-i];

            if (negative)
            {
                buf[--charPos] = '-';
            }

            return new String(buf, charPos, (33 - charPos));
        }

        public static int parseInt(String s, int radix)
        {
            //TODO DIGIT mais inutile
            if (s == null)
            {
                return 0;
            }

            if (radix < MIN_RADIX)
            {
                return 0;
            }

            if (radix > MAX_RADIX)
            {
                return 0;
            }

            int result = 0;
            Boolean negative = false;
            int i = 0, max = s.Length;
            int limit;
            int multmin;
            int digit;

            if (max > 0)
            {
                if (s[0] == '-')
                {
                    negative = true;
                    limit = Int32.MinValue;
                    i++;
                }
                else
                {
                    limit = -Int32.MaxValue;
                }
                multmin = limit / radix;
                if (i < max)
                {
                    digit = 0;//Character.digit(s.charAt(i++), radix);
                    if (digit < 0)
                    {
                        return 0;
                    }
                    else
                    {
                        result = -digit;
                    }
                }
                while (i < max)
                {
                    // Accumulating negatively avoids surprises near MAX_VALUE
                    digit = 0;//Character.digit(s.charAt(i++), radix);
                    if (digit < 0)
                    {
                        return 0;
                    }
                    if (result < multmin)
                    {
                        return 0;
                    }
                    result *= radix;
                    if (result < limit + digit)
                    {
                        return 0;
                    }
                    result -= digit;
                }
            }
            else
            {
                return 0;
            }
            if (negative)
            {
                if (i > 1)
                {
                    return result;
                }
                else
                {	/* Only got "-" */
                    return 0;
                }
            }
            else
            {
                return -result;
            }
        }

        public static String toHexString(int i)
        {
            return i.ToString("X");
        }


        

    }
}
