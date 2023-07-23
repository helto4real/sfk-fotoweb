using FotoApi.Features.HandleStBilder.Commands;
using FotoApi.Features.HandleStBilder.Dto;
using FotoApi.Features.HandleStBilder.Queries;
using FotoApi.Infrastructure.Api;
using FotoApi.Infrastructure.ExceptionsHandling;
using FotoApi.Infrastructure.Pipelines;
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

        group.MapDelete("/{id:guid}", async Task<Results<Ok, NotFound<ErrorDetail>>> 
            (Guid id, DeleteStBildHandler handler, FotoAppPipeline pipe, CancellationToken ct) =>
        {
            await pipe.Pipe(new DeleteStBildRequest(id), handler.Handle, ct);
            return TypedResults.Ok();
        });        

        group.MapPut("/{id:guid}", async Task<Results<Ok, NotFound<ErrorDetail>>> 
            (StBildRequest request, UpdateStBildHandler handler, FotoAppPipeline pipe, CancellationToken ct) =>
        {
            await pipe.Pipe(request, handler.Handle, ct);
            return TypedResults.Ok();
        });         
        
        group.MapGet("/{id:guid}", async Task<Results<Ok<StBildResponse>, NotFound<ErrorDetail>>> 
            (Guid id,  GetStBildHandler handler, FotoAppPipeline pipe, CancellationToken ct) =>
        {
            var result =await pipe.Pipe(new GetStBildRequest(id), handler.Handle, ct);
            return TypedResults.Ok(result);
        });        
        
        group.MapGet("/{showPackagedImages:bool}", async Task<Results<Ok<List<StBildResponse>>, NotFound<ErrorDetail>>> 
            (bool showPackagedImages, GetAllStBilderForCurrentUserHandler handler, FotoAppPipeline pipe, CancellationToken ct) =>
        {
            var result =await pipe.Pipe(new GetAllStBilderForCurrentUserRequest(showPackagedImages), handler.Handle, ct);
            return TypedResults.Ok(result);
        });

        group.MapGet("/all/{showPackagedImages}", async Task<Results<Ok<List<StBildResponse>>, NotFound<ErrorDetail>>> 
            (bool showPackagedImages, GetAllStBilderHandler handler, FotoAppPipeline pipe, CancellationToken ct) =>
        {
            var response = await pipe.Pipe(showPackagedImages, handler.Handle, ct);
            return TypedResults.Ok(response);
        });
        
        group.MapPost("/{stBildId}/acceptstatus/{stBildIsAccepted}", async Task<Results<
            Ok, BadRequest<ErrorDetail>, NotFound<ErrorDetail>>> 
            ([FromRoute] Guid stBildId, [FromRoute] bool stBildIsAccepted, 
                AcceptStBildHandler handler, FotoAppPipeline pipe, CancellationToken ct) =>
        {
            await pipe.Pipe(new AcceptStBildRequest(StBildId: stBildId, StBildAcceptStatus: stBildIsAccepted), handler.Handle, ct);
            return TypedResults.Ok();
        });
        
        group.MapGet("/packageble", async Task<Results<Ok<List<StBildResponse>>, NotFound<ErrorDetail>>> 
            (GetAllPackageStBilderQueryHandler handler, FotoAppPipeline pipe, CancellationToken ct) =>
        {
            var result = await pipe.Pipe(handler.Handle, ct);
            return TypedResults.Ok(result);
        });
        
        group.MapPost("/package", async Task<Results<
            Ok, BadRequest<ErrorDetail>, NotFound<ErrorDetail>>> 
            (PackageStBildResquest request, PackageStBilderHandler handler, FotoAppPipeline pipe, CancellationToken ct) =>
        {
            await pipe.Pipe(new PackageStBilderRequest(request.Ids), handler.Handle, ct);
            return TypedResults.Ok();
        });
        
        group.MapPost("{stBildId:guid}/acceptstatus/{stBildAcceptStatus:bool}", async Task<Results<
            Ok, NotFound<ErrorDetail>>> 
            (
                Guid stBildId, 
                bool stBildAcceptStatus, 
                AcceptStBildHandler handler, 
                FotoAppPipeline pipe, 
                CancellationToken ct) =>
        {
            await pipe.Pipe(new AcceptStBildRequest(stBildId, stBildAcceptStatus), handler.Handle, ct);
            return TypedResults.Ok();
        });
        
     
        return group;
    }
}
