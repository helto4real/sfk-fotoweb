using Microsoft.AspNetCore.Authentication;
// using Yarp.ReverseProxy.Forwarder;
//
// namespace Foto.Web.Server;
//
// public static class PublicApi
// {
//     public static RouteGroupBuilder MapPublic(this IEndpointRouteBuilder routes, string todoUrl)
//     {
//         var group = routes.MapGroup("/public");
//
//
//         // Use this HttpClient for all proxied requests
//         var client = new HttpMessageInvoker(new SocketsHttpHandler());
//
//         group.Map("{*path}", async (IHttpForwarder forwarder, HttpContext context) =>
//         {
//             var error = await forwarder.SendAsync(context, todoUrl, client);
//
//             return Results.Empty;
//         });
//
//         return group;
//     }
// }