using System;
using System.Security.Cryptography;
using System.Text;

class PasswordHash
{
    // Function to hash a password
    public static string HashPassword(string password, string salt)
    {
        using (var pbkdf2 = new Rfc2898DeriveBytes(password, Encoding.UTF8.GetBytes(salt), 10000)) // 10,000 iterations
        {
            byte[] hash = pbkdf2.GetBytes(32); // 32 bytes for the hash
            return Convert.ToBase64String(hash); // Convert to base64 string for storage
        }
    }

    // Function to verify a password
    public static bool VerifyPassword(string password, string storedHash, string salt)
    {
        string hashedPassword = HashPassword(password, salt);
        return hashedPassword == storedHash; // Compare hashes
    }

    // Function to generate a random salt
    public static string GenerateSalt()
    {
        byte[] saltBytes = new byte[16]; // 16 bytes is a good length for a salt
        using (var rng = new RNGCryptoServiceProvider())
        {
            rng.GetBytes(saltBytes); // Generate random salt
        }
        return Convert.ToBase64String(saltBytes); // Convert to base64 to store easily
    }
}
