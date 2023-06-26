using System.Security.Cryptography;

namespace FotoApi.Features.HandleUrlTokens;

public static class UrlTokenGenerator
{
    public static string GenerateToken(int length = 10)
    {
        var randomBytes = RandomNumberGenerator.GetBytes(length);
        return Convert.ToBase64String(randomBytes);
    }
}