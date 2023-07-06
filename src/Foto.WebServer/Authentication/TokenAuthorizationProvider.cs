using System.Security.Claims;
using Blazored.LocalStorage;
using Foto.WebServer.Dto;
using Foto.WebServer.Services;
using Microsoft.AspNetCore.Components.Authorization;

namespace Foto.WebServer.Authentication;

public class TokenAuthorizationProvider : AuthenticationStateProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILocalStorageService _localStorageService;
    private readonly IUserService _userService;

    public TokenAuthorizationProvider(
        ILocalStorageService localStorageService,
        IUserService userService,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _localStorageService = localStorageService;
        _userService = userService;
        _httpContextAccessor = httpContextAccessor;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (_httpContextAccessor.HttpContext?.User?.Identity is not null)
            // We already signed in using a external provider
            if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
                return new AuthenticationState(_httpContextAccessor.HttpContext.User);
        var accessToken = await _localStorageService.GetItemAsync<string>("accessToken");

        // Start with an empty ClaimsPrincipal and add the user if we find a user from a valid token
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
        if (!string.IsNullOrEmpty(accessToken))
        {
            var user = await _userService.GetUserByAccessTokenAsync(accessToken);
            if (user is not null) claimsPrincipal = new UserClaimPrincipal(user.UserName, user.Email, user.IsAdmin);
            // else token has expired or is invalid
        }

        return new AuthenticationState(claimsPrincipal);
    }

    public async Task MarkUserAsAuthenticated(User user)
    {
        await _localStorageService.SetItemAsync("accessToken", user.Token);
        // await _localStorageService.SetItemAsync("refreshToken", user.RefreshToken);

        var claimsPrincipal = new UserClaimPrincipal(user.UserName, user.Email, user.IsAdmin);

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
    }

    public async Task SetTokenInformation(string token)
    {
        await _localStorageService.SetItemAsync("accessToken", token);
    }

    public async Task MarkUserAsLoggedOut()
    {
        await _localStorageService.RemoveItemAsync("accessToken");

        var emptyClaimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(emptyClaimsPrincipal)));
    }
}