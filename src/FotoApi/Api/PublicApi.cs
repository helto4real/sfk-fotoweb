using FotoApi.Features.HandleUsers.Commands;
using FotoApi.Infrastructure.Api;
using FotoApi.Infrastructure.ExceptionsHandling;
using FotoApi.Model;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FotoApi.Api;

public static class PublicApi
{
    public static RouteGroupBuilder MapPublic(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/public");
        group.WithTags("Public");

        group.MapGet("confirmemail/{token}",
            async Task<Results<Ok, NotFound<ErrorDetail>, BadRequest<ErrorDetail>>> 
                (string token, IMediator mediator) =>
        {
            var urlToken = Uri.UnescapeDataString(token);
            await mediator.Send(new ConfirmEmailCommand(urlToken));
            return TypedResults.Ok();
        });

        return group;
    }
}