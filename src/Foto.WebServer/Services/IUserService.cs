using Foto.WebServer.Dto;

namespace Foto.WebServer.Services;

public interface IUserService
{
    Task<(User?, ErrorDetail?)> LoginAsync(LoginUserInfo user);
    Task<(User?, ErrorDetail?)> RegisterUserAsync(NewUserInfo user);
    Task<User?> GetOrRegisterUserAsync(string provider, ExternalUserInfo userInfo);
    Task<User?> GetUserByAccessTokenAsync(string accessToken);
    // public Task<User> RefreshTokenAsync(RefreshRequest refreshRequest);
    Task<bool> ConfirmEmailAsync(string token);
}