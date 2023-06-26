using FotoApi.Model;

namespace FotoApi.Features.HandleUrlTokens;

public static class UrlTokenCreator
{
    public static UrlToken CreateUrlTokenFromUrlTokenType(UrlTokenType tokenType, string data = "")
    {
        return tokenType switch
        {
            UrlTokenType.ConfirmEmail => new UrlToken
            {
                Id = Guid.NewGuid(),
                Token = UrlTokenGenerator.GenerateToken(),
                ExpirationDate = DateTime.UtcNow + TimeSpan.FromMinutes(20),
                UrlTokenType = tokenType,
                Data = data
            },
            UrlTokenType.ResetPassword => new UrlToken
            {
                Id = Guid.NewGuid(),
                Token = UrlTokenGenerator.GenerateToken(),
                ExpirationDate = DateTime.UtcNow + TimeSpan.FromMinutes(20),
                UrlTokenType = tokenType,
                Data = data
            },
            UrlTokenType.AllowAddImage => new UrlToken
            {
                Id = Guid.NewGuid(),
                Token = UrlTokenGenerator.GenerateToken(),
                ExpirationDate = DateTime.UtcNow + TimeSpan.FromDays(1),
                UrlTokenType = tokenType,
                Data = data
            },
            UrlTokenType.AllowAddUser => new UrlToken
            {
                Id = Guid.NewGuid(),
                Token = UrlTokenGenerator.GenerateToken(),
                ExpirationDate = DateTime.UtcNow + TimeSpan.FromDays(30),
                UrlTokenType = tokenType,
                Data = data
            },
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}