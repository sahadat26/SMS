using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Text;

namespace SMS
{
    public class CommonMethod
    {
        public static bool MatchChar(string input, string tomatch)
        {

            Match m = Regex.Match(input, tomatch,RegexOptions.IgnoreCase);

            return m.Success;
        }
        public static string numToStr(int Num, int StrDigit)
        {
            int SerialLength = Num.ToString().Length;
            int RemainingLength = StrDigit - SerialLength;
            string ZeroPrfix = "";
            for (int i = 0; i < RemainingLength; i++)
            {
                ZeroPrfix = ZeroPrfix + "0";
            }

            return ZeroPrfix + Num.ToString();
        }
        public static string GetMD5(string value)
        {
            MD5 algorithm = MD5.Create();
            byte[] data = algorithm.ComputeHash(Encoding.UTF8.GetBytes(value));
            string sh1 = "";
            for (int i = 0; i < data.Length; i++)
            {
                sh1 += data[i].ToString("x2").ToUpperInvariant();
            }
            return sh1;
        }
    }
}