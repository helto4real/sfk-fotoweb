using System.Security.Claims;

namespace Foto.WebServer.Authentication;

public class UserClaimPrincipal : ClaimsPrincipal

{
    public UserClaimPrincipal(string username, string email, bool isAdmin)
        : base(CreateClaimIdentity(username, email, isAdmin))
    {
    }

    private static ClaimsIdentity CreateClaimIdentity(string username, string email, bool isAdmin)
    {
        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.NameIdentifier, username),
            new Claim(ClaimTypes.Email, email)
        }, "fotowebb authentication");

        if (isAdmin)
            identity.AddClaim(new Claim(ClaimTypes.Role, "Admin"));
        else
            identity.AddClaim(new Claim(ClaimTypes.Role, "User"));
        return identity;
    }
}