namespace FotoApi.Infrastructure.Security.Authentication.Dto;

public record RefreshTokenRequest(string RefreshToken, string UserName);