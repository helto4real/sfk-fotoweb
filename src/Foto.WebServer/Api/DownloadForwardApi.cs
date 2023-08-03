using System.Net.Http.Headers;
using Foto.WebServer.Authentication;
using Foto.WebServer.Services;
using Microsoft.AspNetCore.Authentication;
using Yarp.ReverseProxy.Forwarder;

namespace Foto.WebServer.Api;

public static class DownloadForwardApi
{
    public static RouteGroupBuilder MapDownloadForwardApi(this IEndpointRouteBuilder routes, string todoUrl)
    {
        var group = routes.MapGroup("/api/downloads");

        group.RequireAuthorization();

        var transform = static async ValueTask (HttpContext context, HttpRequestMessage req) =>
        {
            var authService = context.RequestServices.GetRequiredService<IAuthService>();

            var result = await context.AuthenticateAsync();

            var properties = result.Properties!;

            var refreshToken = properties.GetTokenValue(TokenNames.AccessToken);
            
            if (refreshToken is null || result.Principal?.Identity?.Name is null) return;
            
            var (accessToken, _) = authService.GetAccessTokenFromRefreshToken(refreshToken, result.Principal!.Identity!.Name!);
            
            if (accessToken is null) return;
            
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            if (properties.HasExternalToken() && properties.GetExternalProvider() is { } externalProvider)
                // Set the external provider name as the scheme so we can do auth
                // on the backend with the right configuration
                req.Headers.TryAddWithoutValidation("X-Auth-Scheme", externalProvider);
        };

        // Use this HttpClient for all proxied requests
        var client = new HttpMessageInvoker(new SocketsHttpHandler());

        group.Map("{*path}", async (IHttpForwarder forwarder, HttpContext context, ILogger<StBildService> logger) =>
        {
            context.Response.OnCompleted(async () =>
            {
                if (context.Response.StatusCode == StatusCodes.Status200OK && !context.RequestAborted.IsCancellationRequested)
                {
                    
                    if (context.Request.Path.Value?.StartsWith("/api/downloads/stpackage") == true)
                    {
                        var idSegment = context.Request.Path.Value?.Split('/').LastOrDefault();
                        if (Guid.TryParse(idSegment, out var id))
                        {
                            var stBildService = context.RequestServices.GetRequiredService<IStBildService>();
                            var result = await stBildService.SetPackageStatusDelivered(id);
                            if (result is not null)
                            {
                                logger.LogError("Failed to set package status to delivered: {Error}", result);
                            }
                        }
                    }
                }
            });
            await forwarder.SendAsync(context, todoUrl, client, transform);
            
            return Results.Empty;
        });

        return group;
    }
}