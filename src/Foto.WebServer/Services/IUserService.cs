namespace Foto.WebServer.Services;

public interface IUserService
{
    Task<bool> ConfirmEmailAsync(string token);
}