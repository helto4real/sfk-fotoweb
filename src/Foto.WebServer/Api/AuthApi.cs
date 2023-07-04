using System.Security.Claims;
using Blazored.LocalStorage;
using Foto.WebServer.Authentication;
using Foto.WebServer.Dto;
using Foto.WebServer.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Foto.WebServer.Api;

public static class AuthApi
{
    public static RouteGroupBuilder MapAuth(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/auth");

        group.MapPost("logout", (HttpContext context) =>
        {
            return Results.SignOut(
                properties: new AuthenticationProperties { RedirectUri = "/login" },
                authenticationSchemes: new[] { CookieAuthenticationDefaults.AuthenticationScheme }
            );
        });
        
        // External login
        group.MapGet("login/{provider}", (string provider, HttpContext context) =>
        {
            var urlToken = context.Request.Query["token"].Count == 1 ? context.Request.Query["token"][0] : string.Empty;

            var redirectUrl = string.IsNullOrEmpty(urlToken)
                ? $"/api/auth/signin/{provider}"
                : $"/api/auth/signin/{provider}?token={Uri.EscapeDataString(urlToken)}";
            
            // Trigger the external login flow by issuing a challenge with the provider name.
            // This name maps to the registered authentication scheme names in AuthenticationExtensions.cs
            return Results.Challenge(
                properties: new() { RedirectUri = redirectUrl },
                authenticationSchemes: new[] { provider }
                );
        });

        group.MapGet("signin/{provider}", async (string provider, IUserService client, 
            HttpContext context, 
            AuthenticationStateProvider authenticationStateProvider,
            ILocalStorageService localStorageService) =>
        {
            var urlToken = context.Request.Query["token"].Count == 1 ? context.Request.Query["token"][0] : string.Empty;

            if (!string.IsNullOrEmpty(urlToken))
            {
                urlToken = Uri.UnescapeDataString(urlToken);
            }
            
            // Grab the login information from the external login dance
            var result = await context.AuthenticateAsync(AuthConstants.ExternalScheme);

            if (result.Succeeded)
            {
                var principal = result.Principal;

                var id = principal.FindFirstValue(ClaimTypes.NameIdentifier)!;

                // TODO: We should have the newUser pick a newUser name to complete the external login dance
                // for now we'll prefer the email address
                var name = (principal.FindFirstValue(ClaimTypes.Email) ?? principal.Identity?.Name)!;
                
                var token = await client.GetOrCreateUserAsync(provider, new() { Username = name, ProviderKey = id, UrlToken = urlToken ?? string.Empty});

                if (token is not null)
                {
                    var authStateProvider = (TokenAuthorizationProvider)authenticationStateProvider;
                    var externalClaimsIdentity = authStateProvider.GetExternalClaimsIdentity(id, name, token.IsAdmin );
                    var authTokens = result.Properties.GetTokens();
                    var properties = new AuthenticationProperties();
                    properties.SetExternalProvider(provider);
                    if (authTokens.Any())
                    {
                        properties.SetHasExternalToken(true);
                    }
                    var tokens = authTokens.Any() ? authTokens : new[]
                    {
                        new AuthenticationToken { Name = TokenNames.AccessToken, Value = token.Token}
                    };
                    properties.StoreTokens(tokens);
                    var claimPrincipal = new ClaimsPrincipal(externalClaimsIdentity);
                    // We create an temporary cookie to store the token so
                    // we can use it to authenticate in the provider correctly
                    context.Response.Cookies.Append("externaltokeninfo", token.Token, 
                        new CookieOptions(){Secure = true, MaxAge = TimeSpan.FromSeconds(10)});
                    await Results.SignIn(claimPrincipal,
                        properties: properties,
                        authenticationScheme: CookieAuthenticationDefaults.AuthenticationScheme).ExecuteAsync(context);
                    
                }
                else
                {
                    // The user is not pre-registered, so we need to move them back to the pre-registration page
                    
                    // Delete the external cookie
                    await context.SignOutAsync(AuthConstants.ExternalScheme);
                    return Results.Redirect($@"/register/?token={Uri.EscapeDataString(urlToken??string.Empty)}&success=false");
                    
                }
            }

            // Delete the external cookie
            await context.SignOutAsync(AuthConstants.ExternalScheme);

            // TODO: Handle the failure somehow

            return Results.Redirect("/");
        });

        return group;
    }

    private static IResult SignIn(LoginUserInfo newUserInfo, AuthToken token)
    {
        return SignIn(newUserInfo.Username, newUserInfo.Username, token, providerName: null, authTokens: Enumerable.Empty<AuthenticationToken>());
    }

    private static IResult SignIn(string userId, string userName, AuthToken token, string? providerName, IEnumerable<AuthenticationToken> authTokens)
    {
        var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userId));
        identity.AddClaim(new Claim(ClaimTypes.Name, userName));
        if (token.IsAdmin)
        {
            identity.AddClaim(new Claim(ClaimTypes.Role, "Admin"));
        }
        var properties = new AuthenticationProperties();

        // Store the external provider name so we can do remote sign out
        if (providerName is not null)
        {
            properties.SetExternalProvider(providerName);
        }

        if (authTokens.Any())
        {
            properties.SetHasExternalToken(true);
        }

        // properties.AddCookieExpiration(TimeSpan.FromSeconds(10));

        var tokens = authTokens.Any() ? authTokens : new[]
        {
            new AuthenticationToken { Name = TokenNames.AccessToken, Value = token.Token }
        };

        properties.StoreTokens(tokens);


        return Results.SignIn(new ClaimsPrincipal(identity),
            properties: properties,
            authenticationScheme: CookieAuthenticationDefaults.AuthenticationScheme);
    }
}
