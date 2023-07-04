
using Blazored.LocalStorage;
using Foto.WebServer.Dto;
using Foto.WebServer.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Foto.WebServer.Authentication;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;

public class TokenAuthorizationProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorageService;
    private readonly IUserService _userService;
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TokenAuthorizationProvider(
        ILocalStorageService localStorageService,
        IUserService userService,
        HttpClient httpClient,
        IHttpContextAccessor httpContextAccossor
        )
    {
        _localStorageService = localStorageService;
        _userService = userService;
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccossor;
    }
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (_httpContextAccessor.HttpContext?.User?.Identity is not null)
        {
            // We already signed in using a external provider
            if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                return new AuthenticationState(_httpContextAccessor.HttpContext.User);
            }
        }
        var accessToken = await _localStorageService.GetItemAsync<string>("accessToken");
        
        ClaimsIdentity? identity = null;

        if (!string.IsNullOrEmpty(accessToken))
        {
            var user = await _userService.GetUserByAccessTokenAsync(accessToken);
            if (user is not null)
            {
                identity = GetClaimsIdentity(user!);
            }
            // else token has expired or is invalid
        }

        var claimsPrincipal = new ClaimsPrincipal(identity ?? new ClaimsIdentity());
        return new AuthenticationState(claimsPrincipal);
    }
    
    public async Task MarkUserAsAuthenticated(User user)
    {
        await _localStorageService.SetItemAsync("accessToken", user.Token);
        // await _localStorageService.SetItemAsync("refreshToken", user.RefreshToken);

        var identity = GetClaimsIdentity(user);

        var claimsPrincipal = new ClaimsPrincipal(identity);

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
    }
    
    public async Task SetTokenInformation(string token)
    {
        await _localStorageService.SetItemAsync("accessToken", token);
    }
    
    public async Task MarkUserAsLoggedOut()
    {
        await _localStorageService.RemoveItemAsync("accessToken");

        var identity = new ClaimsIdentity();

        var user = new ClaimsPrincipal(identity);
        
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    private ClaimsIdentity GetClaimsIdentity(User user)
    {
        var identity = new ClaimsIdentity(new []
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.NameIdentifier, user.UserName),
            new Claim(ClaimTypes.Email, user.Email),    
        }, "fotowebb authentication");

        // identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.UserName));
        // identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
        if (user.IsAdmin)
        {
            identity.AddClaim(new Claim(ClaimTypes.Role, "Admin"));
        }
        else
        {
            identity.AddClaim(new Claim(ClaimTypes.Role, "User"));
        }
        return identity;
    }

    public ClaimsIdentity GetExternalClaimsIdentity(string id, string name, bool isAdmin)
    {
        var identity = new ClaimsIdentity(new []
        {
            new Claim(ClaimTypes.Name, name),
            new Claim(ClaimTypes.NameIdentifier, id),
        }, "fotowebb external authentication");

        if (isAdmin)
        {
            identity.AddClaim(new Claim(ClaimTypes.Role, "Admin"));
        }
        return identity;
    }

    public ValueTask<string> GetToken() => _localStorageService.GetItemAsync<string>("accessToken");
}