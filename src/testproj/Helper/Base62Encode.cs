using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace testsite.Helper
{
    public static class Base62Encode
    {
        public static string alphabet = "0123456789"
            + "ABCDEFGHIJKLMNOPQRSTUVWXYZ" 
            + "abcdefghijklmnopqrstuvwxyz";

        public static string Encode(long input)
        {
            string ret = string.Empty;
            int targetBase = alphabet.Length;
            do
            {
                ret = string.Format("{0}{1}",
                    alphabet[(int)(input % targetBase)],
                    ret);
                input /= targetBase;
            } while (input > 0);
            return ret;
        }
        public static long Decode(string input)
        {
            int srcBase = alphabet.Length;
            long id = 0;
            var charArray = input.ToCharArray();
            Array.Reverse(charArray);
            string ret = new string(charArray);


            for (int i = 0; i < ret.Length; i++)
            {
                long charIndex = alphabet.IndexOf(ret[i]);
                id += charIndex * (long)Math.Pow(srcBase, i);
            }
            return id;
        }
    }
}
