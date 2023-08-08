using Foto.WebServer.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Foto.WebServer.Authentication;

/// <summary>
///     Middleware to refresh the access token from refreshtoken
/// </summary>
public class RefreshTokenMiddleware(RequestDelegate next, ILogger<RefreshTokenMiddleware> logger)
{
    private static readonly string[] ExcludedPaths = { "/login", "/register", "/confirm-email", "/signalr", "/_framework", "/_blazor", "/api/auth" };

    public async Task Invoke(HttpContext context, IAuthService authService)
    {
        if (ExcludedPaths.Any(s => context.Request.Path.StartsWithSegments(s)))
        {
            // We need to exclude some routes from this middleware
            await next(context);
            return;
        }
        
        if (context.User.Identity is not null && context.User.Identity.IsAuthenticated)
        {
            // var expiry = context.User.Claims.FirstOrDefault(x => x.Type == "expires").Value;
            var authResult = await context.AuthenticateAsync();

            var properties = authResult.Properties!;

            var refreshToken = properties.GetTokenValue(TokenNames.AccessToken);

            if (refreshToken is null)
            {
                logger.LogDebug("No refresh token found for user {User} accessing {Path}, redirect to login page", context.User.Identity.Name, context.Request.Path);
                await InvalidateUserAndRedirectToLogin(context);
                return;
            }

            var (accessToken, _) =
                authService.GetAccessTokenFromRefreshToken(refreshToken, context.User.Identity.Name!);

            if (accessToken is not null)
            {
                // We still have a valid access token, continue
                await next(context);
                return;
            }

            // We have an invalid access token, check if we have a refresh token
            var (authInfo, error) = await authService.RefreshAccessTokenAsync(refreshToken, context.User.Identity.Name!);

            if (authInfo is null)
            {
                logger.LogDebug("Could not refresh accesstoken from refresh for {User} accessing {Path}, redirect to login page. Error {ErrorDetail}", context.User.Identity.Name, context.Request.Path, error?.Detail);
                await InvalidateUserAndRedirectToLogin(context);
                return;
            }
            logger.LogDebug("Middleware refreshed accesstoken for {User} accessing {Path}", context.User.Identity.Name, context.Request.Path);
            var userClaimsPrincipal = new UserClaimPrincipal(authInfo.UserName, authInfo.Roles, authInfo.RefreshToken );

            // Make sure we keep the status of external login
            var externalProvider = authResult.Properties?.GetExternalProvider();
            if (externalProvider is not null)
            {
                userClaimsPrincipal.AuthenticationProperties.SetExternalProvider(externalProvider);
            }
            
            await context.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, userClaimsPrincipal, userClaimsPrincipal.AuthenticationProperties);
            context.User = userClaimsPrincipal;
            await next(context);
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