using FotoApi.Features.HandleUrlTokens.CommandHandlers;
using FotoApi.Features.HandleUrlTokens.Dto;
using FotoApi.Features.HandleUrlTokens.QueryHandlers;
using FotoApi.Infrastructure.Api;
using FotoApi.Infrastructure.ExceptionsHandling;
using FotoApi.Infrastructure.Pipelines;
using FotoApi.Infrastructure.Security.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FotoApi.Api;

public static class AdminApi
{
    public static RouteGroupBuilder MapAdmin(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/admin");

        group.WithTags("Admin");

        group.RequireAuthorization(pb => pb.RequireAdminUser())
            .AddOpenApiSecurityRequirement();
        
        // Rate limit all of the APIs
        group.RequirePerUserRateLimit();
        
        group.MapPost("token/addtokenbytype", async Task<Results<Ok< UrlTokenResponse>, BadRequest<ErrorDetail>>> 
            (UrlTokenTypeRequest request, AddUrlTokenFromUrlTokenTypeHandler handler, FotoAppPipeline pipe, CancellationToken ct) =>
        {
            var response = await pipe.Pipe(request, handler.Handle, ct);
            return TypedResults.Ok(response);
        });
        
        group.MapDelete("token/{id}", async Task<Results<Ok, BadRequest<ErrorDetail>, NotFound<ErrorDetail>>> 
            (Guid id, DeleteTokenFromIdHandler handler, FotoAppPipeline pipe, CancellationToken ct) =>
        {
            await pipe.Pipe(id, handler.Handle, ct);
            return TypedResults.Ok();
        });
        
        group.MapGet("token/{token}", async Task<Results<Ok<UrlTokenResponse>, BadRequest<ErrorDetail>, NotFound<ErrorDetail>>> 
            (string token, GetUrlTokenByTokenStringHandler handler, FotoAppPipeline pipe, CancellationToken ct) =>
        {
            var response = await pipe.Pipe(token, handler.Handle, ct);
            return TypedResults.Ok(response);
        });
        
        group.MapGet("token/valid-tokens", async Task<Results<Ok<List<UrlTokenResponse>>, BadRequest<ErrorDetail>>> 
            (GetValidUrlTokensHandler handler, FotoAppPipeline pipe, CancellationToken ct) =>
        {
            var response = await pipe.Pipe(handler.Handle, ct);
            return TypedResults.Ok(response);
        });
        
       
        return group;
    }
}