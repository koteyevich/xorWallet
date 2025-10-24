using System.Security.Cryptography;

namespace xorWallet.Helpers;

public class IdGenerator
{
    private static readonly char[] Chars =
        "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

    public static string Generate()
    {
        return RandomNumberGenerator.GetString(Chars, 32);
    }
}