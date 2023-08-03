using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;

namespace Shared.Security;

public static class WebApplicationExtensions
{
    public static WebApplicationBuilder AddAuthorizationPolicies(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthorization(options =>
        {
            options.AddAdminPolicy();
            options.AddMemberPolicy();
            options.AddCompetitionAdministratorsPolicy();
            options.AddStBildAdministratiorPolicy();
        });
        return builder;
    }
    
    private static void AddAdminPolicy(this AuthorizationOptions options)
    {
        options.AddPolicy("AdminPolicy", policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.RequireClaim(ClaimTypes.Role, "Admin");
        });
    }
    
    private static void AddMemberPolicy(this AuthorizationOptions options)
    {
        options.AddPolicy("MemberPolicy", policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.RequireClaim(ClaimTypes.Role, "Member", "Admin");
        });
    }
    
    private static void AddCompetitionAdministratorsPolicy(this AuthorizationOptions options)
    {
        options.AddPolicy("CompetitionAdministratorsPolicy", policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.RequireClaim(ClaimTypes.Role, "CompetitionAdministrator", "Admin");
        });
    }
    
    private static void AddStBildAdministratiorPolicy(this AuthorizationOptions options)
    {
        options.AddPolicy("StBildAdministratiorPolicy", policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.RequireClaim(ClaimTypes.Role, "StbildAdministrator", "Admin");
        });
    }
}