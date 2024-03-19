using System.Security.Cryptography;
using System.Text;

namespace PerformansYonetimSistemi.Helper
{
    public class LoginHelper
    {
        public static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha256.ComputeHash(bytes);

                // Kriptolanmış şifreyi hexadecimal stringe dönüştürün
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    builder.Append(hashBytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
        public bool VerifyPassword(string enteredPassword, string storedPassword)
        {
            string enteredPasswordHash = HashPassword(enteredPassword);
            return string.Equals(enteredPasswordHash, storedPassword);
        }
        public static string Encrypt(string text, string key)
        {
            byte[] textBytes = Encoding.UTF8.GetBytes(text);
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);

            byte[] encryptedBytes = new byte[textBytes.Length];

            for (int i = 0; i < textBytes.Length; i++)
            {
                encryptedBytes[i] = (byte)(textBytes[i] ^ keyBytes[i % keyBytes.Length]);
            }

            return Convert.ToBase64String(encryptedBytes);
        }

        public static string Decrypt(string encryptedText, string key)
        {
            byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);

            byte[] decryptedBytes = new byte[encryptedBytes.Length];

            for (int i = 0; i < encryptedBytes.Length; i++)
            {
                decryptedBytes[i] = (byte)(encryptedBytes[i] ^ keyBytes[i % keyBytes.Length]);
            }

            return Encoding.UTF8.GetString(decryptedBytes);
        }
    }
}
