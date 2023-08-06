using Foto.WebServer.Dto;

namespace Foto.WebServer.Services;

public interface IAuthService
{
    Task<(LoginInfo?, ErrorDetail?)> LoginAsync(LoginUserInfo loginInfo);
    Task<(User?, ErrorDetail?)> RegisterUserAsync(NewUserInfo user);
    Task<LoginInfo?> GetOrRegisterUserAsync(string provider, ExternalUserInfo userInfo);
    (string?, ErrorDetail?) GetAccessTokenFromRefreshToken(string refreshToken, string userName);
    Task<(LoginInfo?, ErrorDetail?)> RefreshAccessTokenAsync(string refreshToken, string userName);
    Task<bool> CompliesToPolicy(string policyName);
}