using Microsoft.AspNetCore.Authorization;

namespace FotoApi.Infrastructure.Security.Authorization.Policies;

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
            policy.RequireRole("Admin");
        });
    }
    
    private static void AddMemberPolicy(this AuthorizationOptions options)
    {
        options.AddPolicy("MemberPolicy", policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.RequireRole("Member", "Admin");
        });
    }
    
    private static void AddCompetitionAdministratorsPolicy(this AuthorizationOptions options)
    {
        options.AddPolicy("CompetitionAdministratorsPolicy", policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.RequireRole("CompetitionAdministrator", "Admin");
        });
    }
    
    private static void AddStBildAdministratiorPolicy(this AuthorizationOptions options)
    {
        options.AddPolicy("StBildAdministratiorPolicy", policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.RequireRole("StbildAdministrator", "Admin");
        });
    }
}