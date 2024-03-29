﻿using System.Net.Http.Headers;
using Foto.WebServer.Authentication;
using Foto.WebServer.Services;
using Microsoft.AspNetCore.Authentication;
using Yarp.ReverseProxy.Forwarder;

namespace Foto.WebServer.Api;

public static class ImageForwardApi
{
    public static RouteGroupBuilder MapImageForwardApi(this IEndpointRouteBuilder routes, string todoUrl)
    {
        var group = routes.MapGroup("/api/images");

        group.RequireAuthorization();
        group.DisableAntiforgery();
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

        group.MapGet("{*path}", async (IHttpForwarder forwarder, HttpContext context) =>
        {
            await forwarder.SendAsync(context, todoUrl, client, transform);

            return Results.Empty;
        });

        return group;
    }
}