using System.Net;
using System.Net.Http.Headers;
using Foto.WebServer.Dto;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;

namespace Foto.WebServer.Services;

public interface ISignInService
{
    Task<HttpResponseMessage> RefreshTokenOnExpired(Func<Task<HttpResponseMessage>> func);
}

public class SignInService(
        IJSRuntime? jsRuntime,
        IHttpContextAccessor accessor,
        IAuthService authService,
        NavigationManager? navigationManager)
    : ISignInService
{
    private readonly IJSRuntime _jsRuntime = jsRuntime ?? throw new ArgumentNullException(nameof(jsRuntime), "No IJSRuntime available");
    private readonly NavigationManager _navigationManager = navigationManager ?? throw new ArgumentNullException(nameof(navigationManager), "NavigationManager is null");
    private readonly HttpContext _httpContext = accessor.HttpContext ?? throw new ArgumentNullException(nameof(accessor), "No HttpContext available");

    public async Task<HttpResponseMessage> RefreshTokenOnExpired(Func<Task<HttpResponseMessage>> func)
    {
        // Call the provided function to get the response
        var response = await func();
        
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            // The token is expired, try to refresh it and call the function again with the new token
            await RefreshTokens();
            
            // Retry call with the new token
            response = await func();
            
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                // The refresh of token did work or user is not authorized for real 
                SignOut();
            }
        }
        return response;
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
        var refreshToken =  result.Properties!.GetTokenValue("access_token");
        if (refreshToken is null)
        {
            SignOut();
            return;
        }
        var (authInfo, _) = await authService.RefreshAccessTokenAsync(refreshToken, _httpContext.User.Identity!.Name!);
        if (authInfo is null)
        {
            return;
        }
        
        var requestPath = $"{_httpContext.Request.Scheme}://{_httpContext.Request.Host}/api/auth/signin?refreshToken={Uri.EscapeDataString(refreshToken!)}";

        await _jsRuntime.InvokeVoidAsync("fetch", requestPath);
        result.Properties!.UpdateTokenValue("access_token", authInfo.RefreshToken);
        _httpContext.User = result.Principal!;
                    
    }
    private void SignOut()
    {
        _navigationManager.NavigateTo("/signout", true);
    }
}
