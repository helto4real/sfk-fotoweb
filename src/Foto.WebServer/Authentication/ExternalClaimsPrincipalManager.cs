using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace Foto.WebServer.Authentication;

/// <summary>
///     Creates a new ClaimsPrincipal from the external login
/// </summary>
public class ExternalClaimsPrincipalManager
{
    private readonly ClaimsPrincipal _externalPrincipal;
    private readonly AuthenticationProperties _externalProperties;

    public ExternalClaimsPrincipalManager(AuthenticateResult authResult, string provider)
    {
        _ = authResult.Principal ?? throw new NullReferenceException("authResult.Principal is null");
        _ = authResult.Properties ?? throw new NullReferenceException("authResult.Principal is null");
        _externalPrincipal = authResult.Principal;
        _externalProperties = authResult.Properties;
        AuthenticationProperties.SetExternalProvider(provider);
    }

    public AuthenticationProperties AuthenticationProperties { get; } = new();

    public string NameIdentifier => _externalPrincipal.FindFirstValue(ClaimTypes.NameIdentifier)!;
    public string Name => _externalPrincipal.FindFirstValue(ClaimTypes.Email) ?? _externalPrincipal.Identity?.Name!;

    public ClaimsPrincipal NewClaimsPrincipal(string token, bool isAdmin)
    {
        var claimsIdentity = CreateExternalClaimsIdentity(isAdmin);
        AddTokens(token);
        return new ClaimsPrincipal(claimsIdentity);
    }

    public ClaimsIdentity CreateExternalClaimsIdentity(bool isAdmin)
    {
        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, Name),
            new Claim(ClaimTypes.NameIdentifier, NameIdentifier)
        }, "fotowebb external authentication");

        if (isAdmin) identity.AddClaim(new Claim(ClaimTypes.Role, "Admin"));
        return identity;
    }

    private void AddTokens(string token)
    {
        var externalTokens = _externalProperties.GetTokens().ToArray();

        if (externalTokens.Any()) AuthenticationProperties.SetHasExternalToken(true);
        var tokens = externalTokens.Any()
            ? externalTokens
            : new[]
            {
                new AuthenticationToken { Name = TokenNames.AccessToken, Value = token }
            };
        AuthenticationProperties.StoreTokens(tokens);
    }
}