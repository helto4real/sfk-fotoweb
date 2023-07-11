namespace FotoApi.Infrastructure.Security.Authentication.Model;

public record RefreshTokenRequest(string RefreshToken, string UserName);