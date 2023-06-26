using System.Collections.Concurrent;
using System.Security.Claims;
using System.Security.Principal;
using Foto.Web.Client.ApiClients;

namespace Foto.Web.Client.Authorization;

public class AuthorizationManager
{
    private readonly ConcurrentBag<AuthorizedUserInfo> _userAuthInfoBag = new();
    private readonly UserApiClient _client;
    private string[] _socialProviders = Array.Empty<string>();

    public AuthorizationManager(UserApiClient client)
    {
        _client = client;
    }

    public AuthorizedUserInfo? AuthorizedUserInfo => _userAuthInfoBag.IsEmpty ? null : _userAuthInfoBag.First();

    public string[] SocialProviders => _socialProviders;
    
    public void SetUserAuthorizationInfo(AuthorizedUserInfo? userAuthorizationInfo)
    {
        if (_userAuthInfoBag.IsEmpty == false)
            _userAuthInfoBag.Clear();
        
        if (userAuthorizationInfo is not null)
            _userAuthInfoBag.Add(userAuthorizationInfo);
    }

    public bool IsAuthorized => AuthorizedUserInfo?.IsAuthorized ?? false;
    
    public bool IsAdmin => AuthorizedUserInfo?.IsInRole("Admin") ?? false;
    
    public string Username => AuthorizedUserInfo?.Username ?? string.Empty;

    public void Clear()
    {
        _userAuthInfoBag.Clear();
    }
    
    public async Task<bool> Login(string username, string password, string urlToken)
    {
        urlToken = Uri.UnescapeDataString(urlToken);
        if (await _client.LoginAsync(username, password, urlToken))
        {
            var authInfo = await _client.GetUserAuthorizationInfoAsync();
            if (authInfo is not null)
            {
                SetUserAuthorizationInfo(authInfo);
                return true;
            }
        }

        return false;
    }
    
    public async Task Logout()
    {
        if (await _client.LogoutAsync())
        {
            Clear();
        }
    }

    public void SetSocialProviders(string[] socialProviders)
    {
        _socialProviders = socialProviders;
    }
}