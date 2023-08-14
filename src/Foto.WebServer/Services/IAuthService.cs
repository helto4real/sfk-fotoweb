using System.Security.Claims;
using System.Security.Principal;
using Foto.WebServer.Dto;

namespace Foto.WebServer.Services;

public interface IAuthService
{
    Task<(AccountInfo?, ErrorDetail?)> LoginAsync(LoginUserInfo loginInfo);
    Task<(User?, ErrorDetail?)> RegisterUserAsync(NewUserInfo user);
    Task<AccountInfo?> GetOrRegisterUserAsync(string provider, ExternalUserInfo userInfo);
    (string?, ErrorDetail?) GetAccessTokenFromRefreshToken(string refreshToken, string userName);
    Task<(AccountInfo?, ErrorDetail?)> RefreshAccessTokenAsync(string refreshToken, string userName);
    Task<bool> CompliesToPolicy(string policyName);

    static string? GetImageUrlFromClaim(ClaimsPrincipal principal)
    {
        return principal.HasClaim(c => c.Type == "image" && !string.IsNullOrEmpty(c.Value))
            ? principal.Claims.First(c => c.Type == "image").Value
            : null;
    }
}