namespace Foto.WebServer.Dto;

public record AccountInfo(
    string UserName,
    string Token,
    DateTime RefreshTokenExpiration,
    string RefreshToken,
    IReadOnlyCollection<string> Roles);
