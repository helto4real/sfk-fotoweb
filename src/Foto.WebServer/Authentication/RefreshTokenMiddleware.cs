using System.Security.Claims;
using Foto.WebServer.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Foto.WebServer.Authentication;

/// <summary>
///     Middleware to refresh the access token from refreshtoken
/// </summary>
public class RefreshTokenMiddleware
{
    private readonly RequestDelegate _next;
    private static readonly string[] ExcludedPaths = { "/login", "/register", "/signalr", "/_framework", "/_blazor", "/api/auth" };
    public RefreshTokenMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, IAuthService authService)
    {
        if (ExcludedPaths.Any(s => context.Request.Path.StartsWithSegments(s)))
        {
            // We need to exclude some routes from this middleware
            await _next(context);
            return;
        }
        
        if (context.User.Identity is not null && context.User.Identity.IsAuthenticated)
        {
            // var expiry = context.User.Claims.FirstOrDefault(x => x.Type == "expires").Value;
            var result = await context.AuthenticateAsync();

            var properties = result.Properties!;

            var refreshToken = properties.GetTokenValue(TokenNames.AccessToken);

            if (refreshToken is null)
            {
                await InvalidateUserAndRedirectToLogin(context);
                return;
            }

            var (accessToken, _) =
                authService.GetAccessTokenFromRefreshToken(refreshToken, context.User.Identity.Name!);

            if (accessToken is not null)
            {
                // We still have a valid access token, continue
                await _next(context);
                return;
            }

            // We have an invalid access token, check if we have a refresh token
            var (authInfo, _) = await authService.RefreshAccessTokenAsync(refreshToken, context.User.Identity.Name!);

            if (authInfo is null)
            {
                await InvalidateUserAndRedirectToLogin(context);
                return;
            }

            var userClaimsPrincipal = new UserClaimPrincipal(authInfo.UserName, authInfo.IsAdmin, authInfo.RefreshToken );
        
            await context.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, userClaimsPrincipal, userClaimsPrincipal.AuthenticationProperties);
            
            await _next(context);
            return;
             
        }
        await InvalidateUserAndRedirectToLogin(context);
    }

    private async Task InvalidateUserAndRedirectToLogin(HttpContext context)
    {
        await context.SignOutAsync();
        context.Response.Redirect("/login");
    }
}