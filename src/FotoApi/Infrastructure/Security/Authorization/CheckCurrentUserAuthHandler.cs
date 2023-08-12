using Microsoft.AspNetCore.Authorization;

namespace FotoApi.Infrastructure.Security.Authorization;

public static class AuthorizationHandlerExtensions
{
    public static AuthorizationBuilder AddCurrentUserHandler(this AuthorizationBuilder builder)
    {
        builder.Services.AddScoped<IAuthorizationHandler, CheckCurrentUserAuthHandler>();
        // builder.AddPolicy("AdminPolicy", policy =>
        // {
        //     policy.RequireRole("admin");
        // });
        return builder;
    }
    public static AuthorizationBuilder AddAdminUserHandler(this AuthorizationBuilder builder)
    {
        builder.Services.AddScoped<IAuthorizationHandler, CheckAdminUserAuthHandler>();
        // builder.AddPolicy("AdminPolicy", policy =>
        // {
        //     policy.RequireRole("admin");
        // });
        return builder;
    }

    // Adds the current user requirement that will activate our authorization handler
    public static AuthorizationPolicyBuilder RequireCurrentUser(this AuthorizationPolicyBuilder builder)
    {
        return builder.RequireAuthenticatedUser()
                      .AddRequirements(new CheckCurrentUserRequirement());
    }
    public static AuthorizationPolicyBuilder RequireAdminUser(this AuthorizationPolicyBuilder builder)
    {
        return builder.RequireAuthenticatedUser()
                      .AddRequirements(new CheckAdminUserRequirement());
    }

    private class CheckCurrentUserRequirement : IAuthorizationRequirement { }
    private class CheckAdminUserRequirement : IAuthorizationRequirement { }

    // This authorization handler verifies that the user exists even if there's
    // a valid token
    private class CheckCurrentUserAuthHandler(CurrentUser currentUser) : AuthorizationHandler<CheckCurrentUserRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CheckCurrentUserRequirement requirement)
        {
            // TODO: Check user if the user is locked out as well
            if (currentUser.User is not null)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }    
    
    private class CheckAdminUserAuthHandler : AuthorizationHandler<CheckAdminUserRequirement>
    {
        private readonly CurrentUser _currentUser;

        public CheckAdminUserAuthHandler(CurrentUser currentUser)
        {
            this._currentUser = currentUser;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CheckAdminUserRequirement requirement)
        {
            
            // TODO: Check user if the user is locked out as well
            if (_currentUser.User is null) return Task.CompletedTask;
            
            if (_currentUser.IsAdmin)
                context.Succeed(requirement);
            else
                context.Fail();
            return Task.CompletedTask;
        }
    }
}
