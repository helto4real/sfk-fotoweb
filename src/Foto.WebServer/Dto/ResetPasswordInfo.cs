namespace Foto.WebServer.Dto;


public record ResetPasswordInfo(string Email, string Token, string NewPassword);