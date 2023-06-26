using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Foto.Web.Server;

public static class AuthApi
{
    public static RouteGroupBuilder MapAuth(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/auth");

        group.MapPost("register", async (NewUserInfo userInfo, FotoApi client) =>
        {
            // Retrieve the access token given the newUser info
            var token = await client.CreateUserAsync(userInfo);

            if (token is null)
            {
                return Results.Unauthorized();
            }

            return SignIn(new LoginUserInfo(){Username = userInfo.UserName, Password = userInfo.Password, IsAdmin = userInfo.IsAdmin}, token);
        });

        group.MapPost("login", async (LoginUserInfo userInfo, FotoApi client) =>
        {
            // Retrieve the access token give the newUser info
            var token = await client.GetTokenAsync(userInfo);

            if (token is null)
            {
                return Results.Unauthorized();
            }

            return SignIn(userInfo, token);
        });

        group.MapPost("logout", async (HttpContext context) =>
        {
            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // TODO: Support remote logout
            // If this is an external login then use it
            //var result = await context.AuthenticateAsync();
            //if (result.Properties?.GetExternalProvider() is string providerName)
            //{
            //    await context.SignOutAsync(providerName, new() { RedirectUri = "/" });
            //}
        })
        .RequireAuthorization();

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

        group.MapGet("signin/{provider}", async (string provider, FotoApi client, HttpContext context) =>
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
                    // Write the login cookie
                    await SignIn(id, name, token, provider, result.Properties.GetTokens()).ExecuteAsync(context);
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
