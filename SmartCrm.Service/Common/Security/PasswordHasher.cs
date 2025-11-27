using System.Security.Cryptography;
using System.Text;

namespace SmartCrm.Service.Common.Security;

public class PasswordHasher
{
    private static readonly string _key = "d9b4f349-17a5-4125-aa1b-197cbc542090";

    public static string GetHash(string password)
    {
        return GenerateHash(password);
    }

    public static bool IsEqual(string registerPassword, string loginPassword)
    {
        return registerPassword == GenerateHash(loginPassword);
    }

    private static string GenerateHash(string password)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            string combined = password + _key;

            byte[] bytes = Encoding.UTF8.GetBytes(combined);
            byte[] hash = sha256.ComputeHash(bytes);

            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte b in hash)
            {
                stringBuilder.Append(b.ToString("x2"));
            }

            return stringBuilder.ToString();
        }
    }
}
