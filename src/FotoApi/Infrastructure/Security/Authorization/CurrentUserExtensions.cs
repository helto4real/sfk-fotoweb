using System.Security.Claims;
using FotoApi.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace FotoApi.Infrastructure.Security.Authorization;

public static class CurrentUserExtensions
{
    public static IServiceCollection AddCurrentUser(this IServiceCollection services)
    {
        services.AddScoped<CurrentUser>();
        services.AddScoped<IClaimsTransformation, ClaimsTransformation>();
        return services;
    }

    private sealed class ClaimsTransformation(CurrentUser currentUser, UserManager<User> userManager) : IClaimsTransformation
    {
        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            // We're not going to transform anything. We're using this as a hook into authorization
            // to set the current user without adding custom middleware.
            currentUser.Principal = principal;

            var loginProvider = principal.FindFirstValue("provider");

            if (principal.FindFirstValue(ClaimTypes.NameIdentifier) is { Length: > 0 } name)
            {
                // Resolve the user manager and see if the current user is a valid user in the database
                // we do this once and store it on the current user.
                currentUser.User = loginProvider is null
                    ? await userManager.FindByNameAsync(name)
                    : await userManager.FindByLoginAsync(loginProvider, name);
            }

            return principal;
        }
    }
}