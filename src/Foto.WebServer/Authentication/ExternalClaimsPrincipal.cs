using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace Foto.WebServer.Authentication;

/// <summary>
///     Creates a new ClaimsPrincipal from the external login
/// </summary>
public class ExternalClaimsPrincipal
{
    private readonly ClaimsPrincipal _externalPrincipal;
    private readonly AuthenticationProperties _externalProperties;

    public ExternalClaimsPrincipal(AuthenticateResult authResult, string provider)
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

    public ClaimsPrincipal NewClaimsPrincipal(string refreshToken, IReadOnlyCollection<string> roles)
    {
        var claimsIdentity = CreateExternalClaimsIdentity(roles);
        AddTokens(refreshToken);
        return new ClaimsPrincipal(claimsIdentity);
    }

    public ClaimsIdentity CreateExternalClaimsIdentity(IReadOnlyCollection<string> roles)
    {
        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, Name),
            new Claim(ClaimTypes.NameIdentifier, NameIdentifier)
        }, "fotowebb external authentication");

        foreach (var role in roles)
        {
            identity.AddClaim(new Claim(ClaimTypes.Role, role));
        }
        identity.AddClaim(new Claim(ClaimTypes.Role, "User"));
        return identity;
    }

    private void AddTokens(string refreshToken)
    {
        var externalTokens = _externalProperties.GetTokens().ToArray();
        // Todo should I support external tokens?
        // if (externalTokens.Any()) AuthenticationProperties.SetHasExternalToken(true);
        // var tokens = externalTokens.Any()
        //     ? externalTokens
        //     : new[]
        //     {
        //         new AuthenticationToken { Name = TokenNames.AccessToken, Value = refreshToken }
        //     };
        // For now we only support our own tokens
        var tokens = new[] { new AuthenticationToken { Name = TokenNames.AccessToken, Value = refreshToken } };
            
        AuthenticationProperties.StoreTokens(tokens);
    }
}