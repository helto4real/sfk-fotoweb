using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Foto.WebServer.Authentication;

public class UserClaimPrincipal : ClaimsPrincipal

{
    public AuthenticationProperties AuthenticationProperties { get; } = new();
    
    public UserClaimPrincipal(string username, IReadOnlyCollection<string> roles, string refreshToken)
        : base(CreateClaimIdentity(username, roles))
    {
        var tokens = new[]
        {
            new AuthenticationToken { Name = TokenNames.AccessToken, Value = refreshToken },
        };
        
        AuthenticationProperties.StoreTokens(tokens);
    }

    private static ClaimsIdentity CreateClaimIdentity(string username, IReadOnlyCollection<string> roles)
    {
        var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
        
        identity.AddClaims(
            new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.NameIdentifier, username),
            });

        foreach (var role in roles)
        {
            identity.AddClaim(new Claim(ClaimTypes.Role, role));
        }
        identity.AddClaim(new Claim(ClaimTypes.Role, "User"));
        return identity;
    }
}