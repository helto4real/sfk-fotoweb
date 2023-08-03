namespace Foto.WebServer.Dto;

public record LoginInfo(
    string UserName,
    string Token,
    DateTime RefreshTokenExpiration,
    string RefreshToken,
    IReadOnlyCollection<string> Roles);
