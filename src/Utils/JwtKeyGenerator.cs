using System.Security.Cryptography;

namespace src.Utils;

public class JwtKeyGenerator
{
    public static string GenerateJwtKey()
    {
        var key = new Byte[32];

        using (var generator = RandomNumberGenerator.Create())
        {
            generator.GetBytes(key);
        }

        return Convert.ToBase64String(key);
    }
}
