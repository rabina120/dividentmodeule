using System;
using System.Text;

namespace Repository.Helper
{
    public class AlphaNumericString
    {
        public static string getAlphaNumericString(int n)
        {
            string AlphaNumericString = "ABCDEFGHIJKLMNOPQRSTUVWXYZ" + "0123456789" + "abcdefghijklmnopqrstuvxyz";
            StringBuilder sb = new StringBuilder(n);
            Random rnd = new Random();
            for (int i = 0; i < n; i++)
            {
                var random = Math.Round(new decimal(rnd.NextDouble()), 5);
                int index = (int)(AlphaNumericString.Length * random);
                sb.Append(AlphaNumericString[index]);
            }
            return sb.ToString();
        }
    }
}
