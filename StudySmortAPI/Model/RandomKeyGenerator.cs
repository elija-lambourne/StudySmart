using System.Security.Cryptography;

namespace StudySmortAPI.Model;

public static class RandomKeyGenerator
{
    public static byte[] Generate256BitKey()
    {
        byte[] key = new byte[32]; // 256 bits = 32 bytes
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(key);
        }
        return key;
    }

    public static string Generate256BitKeyString()
    {
        byte[] key = Generate256BitKey();
        return Convert.ToBase64String(key);
    }
}
