using System.Security.Cryptography;
using System.Text;

namespace StoreManager.Extensions
{
    public static class PasswordHelper
    {
        public static string HashPassword(string password, string salt)
        {
            var combinedString = password + salt;

            using (var sha512 = SHA512.Create())
            {
                var combinedBytes = Encoding.Unicode.GetBytes(combinedString); // SQL NVARCHAR uses UTF-16 (Unicode)
                var hashBytes = sha512.ComputeHash(combinedBytes);

                // Convert the hash to a hexadecimal string
                return BitConverter.ToString(hashBytes).Replace("-", "").ToUpper(); // Match SQL's UPPERCASE hex
            }
        }

        public static bool ValidatePassword(string enteredPassword, string storedHash, string salt)
        {
            var hashedInput = HashPassword(enteredPassword, salt);

            return string.Equals(hashedInput, storedHash, StringComparison.OrdinalIgnoreCase);
        }
    }
}