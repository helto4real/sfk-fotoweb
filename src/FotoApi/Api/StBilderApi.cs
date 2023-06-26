using FotoApi.Features.HandleStBilder.Commands;
using FotoApi.Features.HandleStBilder.Dto;
using FotoApi.Features.HandleStBilder.Queries;
using FotoApi.Infrastructure.Api;
using FotoApi.Infrastructure.ExceptionsHandling;
using FotoApi.Infrastructure.Security.Authorization;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using StBildMapper = FotoApi.Features.HandleStBilder.Dto.StBildMapper;

namespace FotoApi.Api;

public static class StBilderApi
{
    public static RouteGroupBuilder MapStBildApi(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/stbilder");

        group.WithTags("StBilder");


        group.RequireAuthorization(pb => pb.RequireCurrentUser())
            .AddOpenApiSecurityRequirement();
        
        // Rate limit all of the APIs
        group.RequirePerUserRateLimit();

        var mapper = new StBildMapper();
        
        // group.MapPost("/",
        //     async Task<Results<Ok<IdentityResponse>, ValidationProblem>> 
        //     (StBildRequest request, CurrentUser owner, IMediator mediator) =>
        //     {
        //         var command = mapper.ToNewStBildCommand(request, owner);
        //         var result = await mediator.Send(command);
        //         return TypedResults.Ok(result);
        //     });
        
        group.MapDelete("/{id:guid}", async Task<Results<Ok, NotFound<ErrorDetail>>> 
            (Guid id, CurrentUser owner, IMediator mediator) =>
        {
            await mediator.Send(new DeleteStBildCommand(id, owner));
            return TypedResults.Ok();
        });        

        group.MapPut("/{id:guid}", async Task<Results<Ok, NotFound<ErrorDetail>>> 
            (Guid id, StBildRequest request, CurrentUser owner, IMediator mediator) =>
        {
            var updateStBildCommand = mapper.ToStBildCommand(request, id);
            await mediator.Send(updateStBildCommand);
            return TypedResults.Ok();
        });         
        group.MapGet("/{id:guid}", async Task<Results<Ok<StBildResponse>, NotFound<ErrorDetail>>> 
            (Guid id, CurrentUser owner, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetStBildQuery(id, owner));
            return TypedResults.Ok(result);
        });        
        
        group.MapGet("/", async Task<Results<Ok<List<StBildResponse>>, NotFound<ErrorDetail>>> 
            (CurrentUser owner, IMediator mediator, HttpContext ctx) =>
        {
            var useOnlyCurrentUserImages = false;
            ctx.Request.Query.TryGetValue("useMyImages", out var useMyImages);
            if (useMyImages == "True")
                useOnlyCurrentUserImages = true;
            
            var result = await mediator.Send(new GetAllStBilderQuery(useOnlyCurrentUserImages, owner));
            return TypedResults.Ok(result);
        });
        
     
        return group;
    }
}
