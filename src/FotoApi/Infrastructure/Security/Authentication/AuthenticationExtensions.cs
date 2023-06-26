using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;

namespace FotoApi.Infrastructure.Security.Authentication;

public static class AuthenticationExtensions
{
    public static WebApplicationBuilder AddAuthentication(this WebApplicationBuilder builder)
    {
        IdentityModelEventSource.ShowPII = true;
        
        builder.Services.AddAuthentication(
        options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(o =>
        {
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey
                    (Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true
            };
        });
        // var authenticationBuilder = builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);
        // // Add the local JWT bearer
        // authenticationBuilder.AddJwtBearer(options =>
        // {
        //     // Let the client choose the scheme via a header. This will 
        //     // redirect to another. If none was specified we'll use our default scheme.
        //     options.ForwardDefaultSelector = context => context.Request.Headers["X-Auth-Scheme"];
        //     options.TokenValidationParameters.ClockSkew = TimeSpan.FromSeconds(10);
        //     // options.TokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(signingKeyBytes);
        // });
        //
        // // An example of what the expected schema looks like
        // // "Authentication": {
        // //     "Schemes": {
        // //       "<scheme>": {
        // //         <options>
        // //       }
        // //     }
        // //   }
        //
        // // TODO: Make this support any configured provider
        // var section = builder.Configuration.GetSection("Authentication:Schemes:Auth0");
        //
        // if (section.Exists())
        // {
        //     // This is what the auth0 configuration looks like:
        //     //{
        //     //    "Authentication": {
        //     //        "Schemes": {
        //     //            "Auth0": {
        //     //                "ValidAudiences": [ "<your audience here>" ],
        //     //                "Authority": "<your authority here>"
        //     //            }
        //     //        }
        //     //    }
        //     //}
        //     authenticationBuilder.AddJwtBearer("Auth0", options =>
        //     {
        //         options.Events = new()
        //         {
        //             OnTokenValidated = context =>
        //             {
        //                 if (context.Principal?.Identity is ClaimsIdentity identity)
        //                 {
        //                     // Add this claim in memory so that we can look up the user by provider name
        //                     identity.AddClaim(new("provider", "Auth0"));
        //                 }
        //
        //                 return Task.CompletedTask;
        //             }
        //         };
        //     });
        // }

        return builder;
    }
}
