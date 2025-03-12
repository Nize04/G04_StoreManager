using System.Security.Cryptography;

public static class PasswordHelper
{
    public static bool ContainsNumber(string password)
    {
        return password.Any(char.IsDigit);
    }

    public static bool ContainsUppercase(string password)
    {
        return password.Any(char.IsUpper);
    }

    public static bool ContainsLowercase(string password)
    {
        return password.Any(char.IsLower);
    }

    public static bool ContainsSpecialCharacter(string password)
    {
        return password.Any(ch => !char.IsLetterOrDigit(ch));
    }


    public static bool IsLengthValid(string password)
    {
        return password.Length >= 8;
    }

    public static bool CheckPasswordRequirements(string password)
    {
        return IsLengthValid(password) &&
               ContainsNumber(password) &&
               ContainsUppercase(password) &&
               ContainsLowercase(password) &&
               ContainsSpecialCharacter(password);
    }
    public static (string Hash, string Salt) HashPassword(string password)
    {
        byte[] saltBytes = new byte[16]; // 16-byte salt
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(saltBytes);
        }

        using (var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, 100000, HashAlgorithmName.SHA256))
        {
            byte[] hashBytes = pbkdf2.GetBytes(32);
            return (BytesToHex(hashBytes), BytesToHex(saltBytes)); // Convert to Hex
        }
    }

    // Validate the entered password
    public static bool ValidatePassword(string enteredPassword, string storedHash, string storedSalt)
    {
        byte[] saltBytes = HexToBytes(storedSalt);

        using (var pbkdf2 = new Rfc2898DeriveBytes(enteredPassword, saltBytes, 100000, HashAlgorithmName.SHA256))
        {
            byte[] hashBytes = pbkdf2.GetBytes(32);
            return BytesToHex(hashBytes) == storedHash; // Compare HEX hash
        }
    }

    // Convert Byte Array to Hex String
    private static string BytesToHex(byte[] bytes) => BitConverter.ToString(bytes).Replace("-", "").ToLower();

    // Convert Hex String to Byte Array
    private static byte[] HexToBytes(string hex)
    {
        int length = hex.Length / 2;
        byte[] bytes = new byte[length];
        for (int i = 0; i < length; i++)
        {
            bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
        }
        return bytes;
    }
}
