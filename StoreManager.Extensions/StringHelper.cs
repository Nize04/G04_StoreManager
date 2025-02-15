using System.Security.Cryptography;
using System.Text;

namespace StoreManager.Extensions
{
    public static class StringHelper
    {
        private const long Multiplier = 5434324234;
        private const long Offset = 4324234345;

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

        public static string[] GetRoles(this string roles)
        {
            if (string.IsNullOrEmpty(roles))
            {
                return new string[0];
            }

            return roles
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(role => role.Trim())
                .ToArray();
        }

        public static long EncodeAccountId(this int accountId)
        {
            return (accountId * Multiplier) + Offset;
        }

        // Method to decode the big number back to accountId
        public static int DecodeAccountId(this long bigNumber)
        {
            long decodedId = (bigNumber - Offset) / Multiplier;
            return (int)decodedId;
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