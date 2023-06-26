namespace FotoApi.Infrastructure.Security.Authentication.Model;

public record LoginUserRequest (string UserName, string Password, bool IsAdmin);
