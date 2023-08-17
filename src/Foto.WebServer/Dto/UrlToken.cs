namespace Foto.WebServer.Dto;

public record UrlToken(Guid Id, string Token, DateTime ExpirationDate, UrlTokenType UrlTokenType);

public enum UrlTokenType
{
    ResetPassword,
    ConfirmEmail,
    AllowAddUser,
    AllowAddImage
}

public record NewTokenByType
{
    public UrlTokenType UrlTokenType { get; init; }
}