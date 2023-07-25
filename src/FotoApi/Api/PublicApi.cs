using FotoApi.Features.HandleUsers.CommandHandlers;
using FotoApi.Infrastructure.ExceptionsHandling;
using FotoApi.Infrastructure.Pipelines;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FotoApi.Api;

public static class PublicApi
{
    public static RouteGroupBuilder MapPublic(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/public");
        group.WithTags("Public");

        group.MapGet("confirmemail/{token}",
            async Task<Results<Ok, NotFound<ErrorDetail>, BadRequest<ErrorDetail>>> 
                (string token, ConfirmEmailHandler handler, FotoAppPipeline pipe, CancellationToken ct) =>
        {
            var urlToken = Uri.UnescapeDataString(token);
            await pipe.Pipe(urlToken, handler.Handle, ct);
            return TypedResults.Ok();
        });
        
        return group;
    }
}