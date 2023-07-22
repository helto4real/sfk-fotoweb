using System.Security.Claims;
using Blazored.LocalStorage;
using Foto.WebServer.Authentication;
using Foto.WebServer.Dto;
using Foto.WebServer.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace Foto.WebServer.Api;

public static class AuthApi
{
    public static RouteGroupBuilder MapAuthenticationApi(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/auth");

        group.MapGet("refresh", async ([FromQuery]string? refreshToken, HttpContext context, IAuthService authService) =>
        {
            // Todo: Make more robust
            var (authInfo, _) = await authService.RefreshAccessTokenAsync(refreshToken!, context.User!.Identity!.Name!);
            if (authInfo is null)
            {
                return Results.BadRequest("Invalid refresh token");
            }
            var userClaimsPrincipal = new UserClaimPrincipal(authInfo.UserName, authInfo.IsAdmin, authInfo.RefreshToken );

            await context.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, userClaimsPrincipal, userClaimsPrincipal.AuthenticationProperties);
            return Results.Ok();
        });
        
        group.MapGet("signin", async ([FromQuery]string? refreshToken, HttpContext context) =>
        {
            var userClaimsPrincipal = new UserClaimPrincipal("tomash277@gmail.com", true, refreshToken! );
            await context.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, userClaimsPrincipal, userClaimsPrincipal.AuthenticationProperties);
            return Results.Ok();
        });
        
        group.MapGet("signout", async (HttpContext context) =>
        {
            // Todo: Make more robust
        
            await context.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
            return Results.Ok();
        });
        
        // This is the endpoint used trigger the challenge for external login
        group.MapGet("login/{provider}", (string provider, HttpContext context) =>
        {
            var urlToken = context.Request.Query["token"].Count == 1 ? context.Request.Query["token"][0] : string.Empty;

            var redirectUrl = string.IsNullOrEmpty(urlToken)
                ? $"/api/auth/signin/{provider}"
                : $"/api/auth/signin/{provider}?token={Uri.EscapeDataString(urlToken)}";

            // Trigger the external login flow by issuing a challenge with the provider name.
            // This name maps to the registered authentication scheme names in AuthenticationExtensions.cs
            return Results.Challenge(
                new AuthenticationProperties { RedirectUri = redirectUrl },
                new[] { provider }
            );
        });

        // This is the endpoint where the external provider will callback to
        group.MapGet("signin/{provider}", async (string provider, IAuthService client,
            HttpContext context) =>
        {
            // The urltoken is used to allow a user to register if they have been pre-registered
            var urlToken = GetUrlTokenFromHttpContextQueryString(context) ?? string.Empty;

            // Grab the login information from the external login dance
            var authenticateResult = await context.AuthenticateAsync(AuthConstants.ExternalScheme);
            var externalClaimsPrincipal = new ExternalClaimsPrincipal(authenticateResult, provider);

            if (authenticateResult.Succeeded)
            {
                var user = await client.GetOrRegisterUserAsync(provider, new ExternalUserInfo
                {
                    UserName = externalClaimsPrincipal.Name,
                    ProviderKey = externalClaimsPrincipal.NameIdentifier,
                    UrlToken = urlToken
                });

                if (user is not null)
                {
                    var claimPrincipal = externalClaimsPrincipal.NewClaimsPrincipal(user.RefreshToken, user.IsAdmin);
                    // // We create an temporary cookie to store the token so
                    // // we can use it to authenticate in the provider correctly
                    // context.Response.Cookies.Append("externaltokeninfo", user.Token,
                    //     new CookieOptions { Secure = true, MaxAge = TimeSpan.FromSeconds(10) });
                    await Results.SignIn(claimPrincipal,
                        externalClaimsPrincipal.AuthenticationProperties,
                        CookieAuthenticationDefaults.AuthenticationScheme).ExecuteAsync(context);
                }
                else
                {
                    // The user is not pre-registered, so we need to move them back to the pre-registration page
                    await context.SignOutAsync(AuthConstants.ExternalScheme);
                    return Results.Redirect(
                        $@"/register/?token={Uri.EscapeDataString(urlToken ?? string.Empty)}&success=false");
                }
            }
            // Delete the external cookie
            await context.SignOutAsync(AuthConstants.ExternalScheme);

            return Results.Redirect("/");
        });

        return group;
    }

    private static string? GetUrlTokenFromHttpContextQueryString(HttpContext context)
    {
        var urlToken = context.Request.Query["token"].Count == 1 ? context.Request.Query["token"][0] : string.Empty;

        if (!string.IsNullOrEmpty(urlToken)) urlToken = Uri.UnescapeDataString(urlToken);

        return urlToken;
    }
}