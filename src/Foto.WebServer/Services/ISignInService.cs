﻿using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using Foto.WebServer.Authentication;
using Foto.WebServer.Dto;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Foto.WebServer.Services;

public interface ISignInService
{
    Task<HttpResponseMessage> RefreshTokenOnExpired(Func<Task<HttpResponseMessage>> func, bool doNotSignOutOnUnauthorized = false);
    Task<bool> ValidateAccessTokenAndRefreshIfNeedAsync(HttpClient httpClient);

    Task<bool> IsCurrentUserExternalAsync();
    Task<(AccountInfo?, ErrorDetail?)> LoginAsync(LoginUserInfo loginUserInfo);
}

public class SignInService(
        IJSRuntime? jsRuntime,
        IHttpContextAccessor accessor,
        IAuthService authService,
        NavigationManager? navigationManager)
    : ISignInService
{
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);
    
    private readonly HttpContext _httpContext = accessor.HttpContext ??
                                                throw new ArgumentNullException(nameof(accessor),
                                                    "No HttpContext available");

    private readonly IJSRuntime _jsRuntime =
        jsRuntime ?? throw new ArgumentNullException(nameof(jsRuntime), "No IJSRuntime available");

    private readonly NavigationManager _navigationManager = navigationManager ??
                                                            throw new ArgumentNullException(nameof(navigationManager),
                                                                "NavigationManager is null");

    public async Task<bool> IsCurrentUserExternalAsync()
    {
        var result = await _httpContext.AuthenticateAsync();
        if (result.Succeeded) return result.Properties.GetExternalProvider() is not null;
        SignOut();
        return false;

    }

    public async Task<bool> ValidateAccessTokenAndRefreshIfNeedAsync(HttpClient httpClient)
    {
        var response =
            await RefreshTokenOnExpired(async () =>
                await httpClient.GetAsync("api/users/token/validate"));
        return response.IsSuccessStatusCode;
    }

    public async Task<HttpResponseMessage> RefreshTokenOnExpired(Func<Task<HttpResponseMessage>> func, bool doNotSignOutOnUnauthorized = false)
    {
        // Call the provided function to get the response
        var response = await func();
        if (response.StatusCode != HttpStatusCode.Unauthorized) return response;
        // The token is expired, try to refresh it and call the function again with the new token
        await RefreshTokens();

        // Retry call with the new token
        response = await func();

        if (response.StatusCode == HttpStatusCode.Unauthorized && !doNotSignOutOnUnauthorized)
            // The refresh of token did work or user is not authorized for real 
            SignOut();

        return response;
    }

    public async Task<(AccountInfo?, ErrorDetail?)> LoginAsync(LoginUserInfo loginUserInfo)
    {
        const string apiAuthLogin = "/api/auth/login";
        var requestPath = apiAuthLogin;
        var result = await _jsRuntime.InvokeAsync<JsonArray>("login", requestPath, loginUserInfo);
        if (result[0] is not null)
        {
            return (result[0].Deserialize<AccountInfo?>(_jsonOptions), null);
        }
        return (null, result[1].Deserialize<ErrorDetail?>(_jsonOptions));
    }
    
    /// <summary>
    ///     Refresh the tokens and sets the current context with the current principal updated with new token
    /// </summary>
    private async Task RefreshTokens()
    {
        var result = await _httpContext.AuthenticateAsync();
        if (!result.Succeeded)
        {
            SignOut();
            return;
        }

        var refreshToken = result.Properties!.GetTokenValue("access_token");
        if (refreshToken is null)
        {
            SignOut();
            return;
        }

        var (authInfo, _) = await authService.RefreshAccessTokenAsync(refreshToken, _httpContext.User.Identity!.Name!);
        if (authInfo is null) return;

        var requestPath =
            $"/api/auth/signin?refreshToken={Uri.EscapeDataString(authInfo.RefreshToken)}";

        await _jsRuntime.InvokeVoidAsync("refreshToken", requestPath);

        result.Properties!.UpdateTokenValue("access_token", authInfo.RefreshToken);
        _httpContext.User = result.Principal!;
    }

    private void SignOut()
    {
        _navigationManager.NavigateTo("/signout", true);
    }
}