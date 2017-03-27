using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Utils;

namespace Tera.Libs.Network
{
    public static class TeraCrypt
    {
        public static bool SOCKET_USE_PROXY = false;
        public static System.Text.Encoding utf_8 = System.Text.Encoding.UTF8;

        private static char[] PUBLIC_KEY_POSSIBLE_CHARS = "#?!+-,;:.@%abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();
        //private final static char[] PUBLIC_KEY_POSSIBLE_CHARS = "123456789".toCharArray();
        private static int PUBLIC_KEY_SIZE = 32;
        private static Random secRandom = new Random();

        public static String GenPublicKey()
        {
            StringBuilder curKey = new StringBuilder();
            for (int i = 0; i < PUBLIC_KEY_SIZE; i++)
            {
                curKey.Append(PUBLIC_KEY_POSSIBLE_CHARS[secRandom.Next(PUBLIC_KEY_POSSIBLE_CHARS.Length)]);
            }
            return curKey.ToString();
        }

        public static String DecryptKeyOfPublicKey(String public_key)
        {
            return Base64.encodeBase64String(new NeoString(PRIVATE_KEY).encrypt(public_key));
        }

        //private final static String PRIVATE_KEY = "V*a;b9N?RrMgbeùoxuS!géf£PNoXV§97eQ²87XYTce§wXûBn+2wâ£X^èU?1mü²Rc";
        private static String PRIVATE_KEY = "²mIl/Jx;4FGKSF^I6!p/K²5/I?²:oA?sC6Cd#g6dsh-Iyr-QR£-Péa+PnR#0W+JGAtp,V!Dl0jèT§M3pfmII²uh?yg+iHchO6l,P;µNRg--T.v9éJz-c:£Y@%SSB,Xµ9";

        public static String toUtf(String s)
        {
            byte[] utf = System.Text.Encoding.UTF8.GetBytes(s);
            return System.Text.Encoding.UTF8.GetString(utf);
        }
    }
}
