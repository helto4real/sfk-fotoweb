// using Microsoft.AspNetCore.Authentication;
// using Yarp.ReverseProxy.Forwarder;
//
// namespace Foto.Web.Server;
//
//
// public static class AdminApi
// {
//     public static RouteGroupBuilder MapAdmin(this IEndpointRouteBuilder routes, string todoUrl)
//     {
//         var group = routes.MapGroup("/admin");
//
//         group.RequireAuthorization("AdminPolicy");
//
//         var transform = static async ValueTask (HttpContext context, HttpRequestMessage req) =>
//         {
//             var result = await context.AuthenticateAsync();
//             var properties = result.Properties!;
//
//             var accessToken = properties.GetTokenValue(TokenNames.AccessToken);
//             req.Headers.Authorization = new("Bearer", accessToken);
//
//             if (properties.HasExternalToken() && properties.GetExternalProvider() is string externalProvider)
//             {
//                 // Set the external provider name as the scheme so we can do auth
//                 // on the backend with the right configuration
//                 req.Headers.TryAddWithoutValidation("X-Auth-Scheme", externalProvider);
//             }
//         };
//
//         // Use this HttpClient for all proxied requests
//         var client = new HttpMessageInvoker(new SocketsHttpHandler());
//
//         group.Map("{*path}", async (IHttpForwarder forwarder, HttpContext context) =>
//         {
//             var error = await forwarder.SendAsync(context, todoUrl, client, transform);
//
//             return Results.Empty;
//         });
//
//         return group;
//     }
// }
