using Foto.WebServer.Dto;

namespace Foto.WebServer.Services;

public interface IUserService
{
    Task<User?> LoginAsync(LoginUserInfo user);
    Task<User?> RegisterUserAsync(NewUserInfo user);
    Task<User?> GetOrCreateUserAsync(string provider, ExternalUserInfo userInfo);
    Task<User?> GetUserByAccessTokenAsync(string accessToken);
    // public Task<User> RefreshTokenAsync(RefreshRequest refreshRequest);
    Task<bool> ConfirmEmailAsync(string token);
}