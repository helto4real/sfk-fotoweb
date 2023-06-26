using System.Security.Claims;
using System.Security.Principal;
using Foto.Web.Client.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Yarp.ReverseProxy.Forwarder;

namespace Foto.Web.Server;

public static class UserApi
{
    public static RouteGroupBuilder MapUser(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/user");

        group.RequireAuthorization();

        group.MapGet("user_authorization", async Task<Results<BadRequest, Ok<AuthorizedUserInfo>>> (HttpContext context ) =>
        {
            // Retrieve the access token given the user info
            var result = await context.AuthenticateAsync();
            var properties = result.Properties!;

            var accessToken = properties.GetTokenValue(TokenNames.AccessToken);
            // return  IResults<AuthorizedUserInfo>.Empty;
            return TypedResults.Ok(new AuthorizedUserInfo(result?.Principal?.Identity?.Name ?? string.Empty, UserRolesExtensions.RolesFromUser(result?.Principal)));
        });
 
        return group;
    }
}

