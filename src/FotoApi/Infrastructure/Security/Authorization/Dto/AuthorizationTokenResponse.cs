namespace FotoApi.Infrastructure.Security.Authorization.Dto;

public record AuthorizationTokenResponse(string Token, bool IsAdmin);