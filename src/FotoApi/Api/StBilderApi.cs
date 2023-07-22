using FotoApi.Features.HandleStBilder.Commands;
using FotoApi.Features.HandleStBilder.Dto;
using FotoApi.Features.HandleStBilder.Queries;
using FotoApi.Infrastructure.Api;
using FotoApi.Infrastructure.ExceptionsHandling;
using FotoApi.Infrastructure.Security.Authorization;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
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
        
        group.MapGet("/{showPackagedImages:bool}", async Task<Results<Ok<List<StBildResponse>>, NotFound<ErrorDetail>>> 
            (bool showPackagedImages, CurrentUser owner, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetAllStBilderForCurrentUserQuery(showPackagedImages, owner));
            return TypedResults.Ok(result);
        });

        group.MapGet("/all/{showPackagedImages}", async Task<Results<Ok<List<StBildResponse>>, NotFound<ErrorDetail>>> 
            (bool showPackagedImages, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetAllStBilderQuery(showPackagedImages));
            return TypedResults.Ok(result);
        });
        
        group.MapPost("/{stBildId}/acceptstatus/{stBildIsAccepted}", async Task<Results<
            Ok, BadRequest<ErrorDetail>, NotFound<ErrorDetail>>> ([FromRoute] Guid stBildId, [FromRoute] bool stBildIsAccepted, IMediator mediator, CurrentUser owner) =>
        {
            await mediator.Send(new AcceptStBildCommand(StBildId: stBildId, StBildAcceptStatus: stBildIsAccepted));
            return TypedResults.Ok();
        });
        
        group.MapGet("/packageble", async Task<Results<Ok<List<StBildResponse>>, NotFound<ErrorDetail>>> 
            (CurrentUser owner, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetAllPackageStBilderQuery(owner));
            return TypedResults.Ok(result);
        });
        
        group.MapPost("/package", async Task<Results<
            Ok<bool>, BadRequest<ErrorDetail>, NotFound<ErrorDetail>>> (PackageStBildResquest request, IMediator mediator, CurrentUser owner) =>
        {
            var result = await mediator.Send(new PackageStBilderCommand(request.Ids, owner));
            return TypedResults.Ok(result);
        });
        group.MapPost("{stBildId:guid}/acceptstatus/{stBildAcceptStatus:bool}", async Task<Results<
            Ok, NotFound<ErrorDetail>>> (Guid stBildId, bool stBildAcceptStatus, PackageStBildResquest request, IMediator mediator, CurrentUser owner) =>
        {
            await mediator.Send(new AcceptStBildCommand(stBildId, stBildAcceptStatus));
            return TypedResults.Ok();
        });
        
     
        return group;
    }
}
