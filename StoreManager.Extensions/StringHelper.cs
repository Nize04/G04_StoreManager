using System.Security.Cryptography;
using System.Text;

namespace StoreManager.Extensions
{
    public static class StringHelper
    {
        public static string Pluralize(this string word)
        {
            if (word.EndsWith("s") || word.EndsWith("sh") || word.EndsWith("ch") || word.EndsWith("x") || word.EndsWith("z"))
            {
                return word + "es";
            }
            if (word.EndsWith("y") && !IsVowel(word[word.Length - 2]))
            {
                return word.Remove(word.Length - 1) + "ies";
            }
            if (word.EndsWith("f") || word.EndsWith("fe"))
            {
                return word.EndsWith("f") ? word.Remove(word.Length - 1) + "ves" : word.Remove(word.Length - 2) + "ves";
            }

            return word + "s";
        }

        public static byte[] HashToken(this string token)
        {
            using (var sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
            }
        }

        private static bool IsVowel(char c) => "aeiouAEIOU".IndexOf(c) != -1;
    }
}