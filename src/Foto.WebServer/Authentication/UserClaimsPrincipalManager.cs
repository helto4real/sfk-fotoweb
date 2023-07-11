using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Foto.WebServer.Authentication;

public class UserClaimPrincipal : ClaimsPrincipal

{
    public AuthenticationProperties AuthenticationProperties { get; } = new();
    
    public UserClaimPrincipal(string username, bool isAdmin, string refreshToken)
        : base(CreateClaimIdentity(username, isAdmin))
    {
        var tokens = new[]
        {
            new AuthenticationToken { Name = TokenNames.AccessToken, Value = refreshToken },
        };
        
        AuthenticationProperties.StoreTokens(tokens);
    }

    private static ClaimsIdentity CreateClaimIdentity(string username, bool isAdmin)
    {
        var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
        
        identity.AddClaims(
            new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.NameIdentifier, username),
            });

        if (isAdmin)
            identity.AddClaim(new Claim(ClaimTypes.Role, "Admin"));
        else
            identity.AddClaim(new Claim(ClaimTypes.Role, "User"));
        return identity;
    }
}