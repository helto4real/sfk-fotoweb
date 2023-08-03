using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace FotoApi.Infrastructure.Security.Authentication;

public static class AuthenticationServiceExtensions
{
    public static IServiceCollection AddTokenService(this IServiceCollection services)
    {
        // Wire up the token service
        return services.AddSingleton<ITokenService, TokenService>();
    }
}

public interface ITokenService
{
    // Generate a JWT token for the specified user name and admin role
    string GenerateToken(string username, IReadOnlyCollection<string> roles);
    (string refreshToken, DateTime expireTime) GenerateRefreshToken();
    string? GetUserIdByAccessTokenAsync(string accessToken);
}

public sealed class TokenService : ITokenService
{
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _refreshTokenValidityInDays;

    private readonly byte[] _key;
    private readonly SigningCredentials _signingCredential;
    private readonly SymmetricSecurityKey _securityKey;

    public TokenService(IConfiguration config)
    {
        _issuer = config["Jwt:Issuer"]!;
        _audience = config["Jwt:Audience"]!;

        if (int.TryParse(config["JWT:RefreshTokenValidityInDays"], out int refreshTokenValidityInDays))
        {
            _refreshTokenValidityInDays = refreshTokenValidityInDays;
        }
        else
        {
            _refreshTokenValidityInDays = 7; // default to 7 days
        }

        _key = Encoding.ASCII.GetBytes
            (config["Jwt:Key"]!);
        _securityKey = new SymmetricSecurityKey(_key);
        _signingCredential = new SigningCredentials
        (_securityKey,
            SecurityAlgorithms.HmacSha512Signature);

    }
    public string GenerateToken(string username, IReadOnlyCollection<string> roles)
    {
         var identity = new ClaimsIdentity(JwtBearerDefaults.AuthenticationScheme);

         identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub, username));

         // REVIEW: Check that this logic is OK for jti claims
         var id = Guid.NewGuid().ToString().GetHashCode().ToString("x", CultureInfo.InvariantCulture);

         identity.AddClaim(new Claim(JwtRegisteredClaimNames.Jti, id));

         foreach (var role in roles)
         {
             identity.AddClaim(new Claim(ClaimTypes.Role, role));
         }

         identity.AddClaim(new Claim(JwtRegisteredClaimNames.Aud,_audience));

         var handler = new JwtSecurityTokenHandler();
         
         var jwtToken = handler.CreateJwtSecurityToken(
             _issuer,
             audience: null,
             identity,
             notBefore: DateTime.UtcNow,
             // expires: DateTime.UtcNow.AddSeconds(5),    // For testing purposes, remember to chage the ClockSkew in AuthenticationExtensions.cs
             expires: DateTime.UtcNow.AddMinutes(15),
             issuedAt: DateTime.UtcNow,
             _signingCredential);

         return handler.WriteToken(jwtToken);
    }

    /// <summary>
    ///     Generates a refresh token and returns it along with the expiration time
    /// </summary>
    /// <remarks>
    ///     For refresh token we just generate an random base64 encoded string. We do not need any other data
    /// </remarks>
    public (string refreshToken, DateTime expireTime) GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        var refreshToken = Convert.ToBase64String(randomNumber);
        return (refreshToken, DateTime.UtcNow.AddDays(_refreshTokenValidityInDays));
    }

    public string? GetUserIdByAccessTokenAsync(string accessToken)
    {
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(accessToken);
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            IssuerSigningKey = _securityKey,
            ValidateIssuer = false,
            ValidateAudience = false
        };

        var principle = handler.ValidateToken(accessToken, tokenValidationParameters, out var securityToken);

        JwtSecurityToken jwtSecurityToken = securityToken as JwtSecurityToken ?? throw new InvalidOperationException("Invalid JWT token");
        
        if (jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase))
        {
            return principle.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        return null;
    }
}
// public sealed class TokenService : ITokenService
// {
//     private readonly string _issuer;
//     private readonly SigningCredentials _jwtSigningCredentials;
//     private readonly Claim[] _audiences;
//
//     public TokenService(IAuthenticationConfigurationProvider authenticationConfigurationProvider)
//     {
//         // We're reading the authentication configuration for the Bearer scheme
//         var bearerSection = authenticationConfigurationProvider.GetSchemeConfiguration(JwtBearerDefaults.AuthenticationScheme);
//
//         // An example of what the expected schema looks like
//         // "Authentication": {
//         //     "Schemes": {
//         //       "Bearer": {
//         //         "ValidAudiences": [ ],
//         //         "ValidIssuer": "",
//         //         "SigningKeys": [ { "Issuer": .., "Value": base64Key, "Length": 32 } ]
//         //       }
//         //     }
//         //   }
//
//         var section = bearerSection.GetSection("SigningKeys:0");
//
//         _issuer = bearerSection["ValidIssuer"] ?? throw new InvalidOperationException("Issuer is not specified");
//         var signingKeyBase64 = section["Value"] ?? throw new InvalidOperationException("Signing key is not specified");
//
//         var signingKeyBytes = Convert.FromBase64String(signingKeyBase64);
//
//         _jwtSigningCredentials = new SigningCredentials(new SymmetricSecurityKey(signingKeyBytes),
//                 SecurityAlgorithms.HmacSha256Signature);
//
//         _audiences = bearerSection.GetSection("ValidAudiences").GetChildren()
//                     .Where(s => !string.IsNullOrEmpty(s.Value))
//                     .Select(s => new Claim(JwtRegisteredClaimNames.Aud, s.Value!))
//                     .ToArray();
//     }
//
//     public string GenerateToken(string username, bool isAdmin = false)
//     {
//         var identity = new ClaimsIdentity(JwtBearerDefaults.AuthenticationScheme);
//
//         identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub, username));
//
//         // REVIEW: Check that this logic is OK for jti claims
//         var id = Guid.NewGuid().ToString().GetHashCode().ToString("x", CultureInfo.InvariantCulture);
//
//         identity.AddClaim(new Claim(JwtRegisteredClaimNames.Jti, id));
//
//         if (isAdmin)
//         {
//             identity.AddClaim(new Claim(ClaimTypes.Role, "Admin"));
//         }
//
//         identity.AddClaims(_audiences);
//
//         var handler = new JwtSecurityTokenHandler();
//
//         var jwtToken = handler.CreateJwtSecurityToken(
//             _issuer,
//             audience: null,
//             identity,
//             notBefore: DateTime.UtcNow,
//             expires: DateTime.UtcNow.AddMinutes(30),
//             issuedAt: DateTime.UtcNow,
//             _jwtSigningCredentials);
//
//         return handler.WriteToken(jwtToken);
//     }
// }
