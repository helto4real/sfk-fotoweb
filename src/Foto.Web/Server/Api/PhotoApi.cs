using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authentication;
using Yarp.ReverseProxy.Forwarder;

namespace Foto.Web.Server;

public static class PhotoApi
{
    private static string todoUrl = string.Empty;
    public static RouteGroupBuilder MapPhotoImages(this IEndpointRouteBuilder routes, string todoUrl)
    {
        var group = routes.MapGroup("/api");
        var publicApi = group.MapGroup("public");
        var stBilderApi = group.MapGroup("stbilder");
        var imagesApi = group.MapGroup("images");
        var users = group.MapGroup("users");
        var adminApi = group.MapGroup("admin");


        // Use this HttpClient for all proxied requests
        var client = new HttpMessageInvoker(new SocketsHttpHandler());

        publicApi.MapPublicGroupAndForwardRequests(todoUrl, client);
        users.MapPublicGroupAndForwardRequests(todoUrl, client);
        imagesApi.MapGroupAndForwardRequests(todoUrl, client)
            .RequireAuthorization();
        stBilderApi.MapGroupAndForwardRequests(todoUrl, client)
            .RequireAuthorization();
        adminApi.MapGroupAndForwardRequests(todoUrl, client)
            .RequireAuthorization("AdminPolicy");  

        return group;
    }

    static RouteGroupBuilder MapGroupAndForwardRequests(this RouteGroupBuilder group, string apiPath, HttpMessageInvoker invoker)
    {
        group.Map("{*path}", async (IHttpForwarder forwarder, HttpContext context) =>
        {
            await forwarder.SendAsync(context, apiPath, invoker, Transform);

            return Results.Empty;
        });
        return group;
    }

    static RouteGroupBuilder MapPublicGroupAndForwardRequests(this RouteGroupBuilder group, string apiPath, HttpMessageInvoker invoker)
    {
        group.Map("{*path}", async (IHttpForwarder forwarder, HttpContext context) =>
        {
            await forwarder.SendAsync(context, apiPath, invoker);

            return Results.Empty;
        });
        return group;
    }
    
    static async ValueTask Transform (HttpContext context, HttpRequestMessage req)
    {
        var result = await context.AuthenticateAsync();
        var properties = result.Properties!;

        var accessToken = properties.GetTokenValue(TokenNames.AccessToken);
        req.Headers.Authorization = new("Bearer", accessToken);

        if (properties.HasExternalToken() && properties.GetExternalProvider() is string externalProvider)
        {
            // Set the external provider name as the scheme so we can do auth
            // on the backend with the right configuration
            req.Headers.TryAddWithoutValidation("X-Auth-Scheme", externalProvider);
        }
    }
}
