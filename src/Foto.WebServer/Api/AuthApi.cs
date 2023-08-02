﻿using System.Globalization;
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

        // Use this function to debug cookie issues DO NOT USE IN PRODUCTION!!!
        // group.MapGet("decrypt", (HttpContext httpContext) =>
        // {
        //     var opt = httpContext.RequestServices
        //         .GetRequiredService<IOptionsMonitor<CookieAuthenticationOptions>>()
        //         .Get(CookieAuthenticationDefaults.AuthenticationScheme); //or use .Get("Cookies")
        //
        //     // TWO - Get the encrypted cookie value
        //     var cookie = opt.CookieManager.GetRequestCookie(httpContext, opt.Cookie.Name);
        //
        //     // THREE - decrypt it
        //     var unprotected =  opt.TicketDataFormat.Unprotect(cookie);
        //     return Results.Ok(unprotected!.Properties.Items);
        // });
        
        group.MapGet("refresh", async ([FromQuery]string? refreshToken, HttpContext context, IAuthService authService) =>
        {
            var (authInfo, _) = await authService.RefreshAccessTokenAsync(refreshToken!, context.User!.Identity!.Name!);
            if (authInfo is null)
            {
                return Results.BadRequest("Invalid refresh token");
            }
            context.Response.Cookies.Append("RefreshTime", DateTime.Now.ToString(CultureInfo.InvariantCulture));
            var userClaimsPrincipal = new UserClaimPrincipal(authInfo.UserName, authInfo.IsAdmin, authInfo.RefreshToken );

            await context.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, userClaimsPrincipal, userClaimsPrincipal.AuthenticationProperties);
            return Results.Ok();
        });
        
        group.MapGet("signin", ([FromQuery]string? refreshToken, HttpContext context, IAuthService authService) =>
        {
            var user = context.User;
            if (!user.Identity!.IsAuthenticated || user.Identity?.Name is null)
            {
                return Results.BadRequest("User is not authenticated");
            }
            
            var userClaimsPrincipal = new UserClaimPrincipal(user.Identity.Name, user.IsInRole("Admin"), refreshToken! );
            return Results.SignIn( userClaimsPrincipal, userClaimsPrincipal.AuthenticationProperties, CookieAuthenticationDefaults.AuthenticationScheme);
        });
        
        group.MapGet("signout", async (HttpContext context) =>
        {
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