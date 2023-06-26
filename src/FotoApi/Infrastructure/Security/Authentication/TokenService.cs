using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
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
    string GenerateToken(string username, bool isAdmin = false);
}

public sealed class TokenService : ITokenService
{
    private readonly string _issuer;
    private readonly string _audience
        ;

    private readonly byte[] _key;
    private readonly SigningCredentials _signingCredential;

    public TokenService(IConfiguration config)
    {
        _issuer = config["Jwt:Issuer"]!;
        _audience = config["Jwt:Audience"]!;
        _key = Encoding.ASCII.GetBytes
            (config["Jwt:Key"]!);
        _signingCredential = new SigningCredentials
        (new SymmetricSecurityKey(_key),
            SecurityAlgorithms.HmacSha512Signature);

    }
    public string GenerateToken(string username, bool isAdmin = false)
    {
         var identity = new ClaimsIdentity(JwtBearerDefaults.AuthenticationScheme);

         identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub, username));

         // REVIEW: Check that this logic is OK for jti claims
         var id = Guid.NewGuid().ToString().GetHashCode().ToString("x", CultureInfo.InvariantCulture);

         identity.AddClaim(new Claim(JwtRegisteredClaimNames.Jti, id));

         if (isAdmin)
         {
             identity.AddClaim(new Claim(ClaimTypes.Role, "Admin"));
         }

         identity.AddClaim(new Claim(JwtRegisteredClaimNames.Aud,_audience));

         var handler = new JwtSecurityTokenHandler();
         
         var jwtToken = handler.CreateJwtSecurityToken(
             _issuer,
             audience: null,
             identity,
             notBefore: DateTime.UtcNow,
             expires: DateTime.UtcNow.AddMinutes(30),
             issuedAt: DateTime.UtcNow,
             _signingCredential);

         return handler.WriteToken(jwtToken);
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
