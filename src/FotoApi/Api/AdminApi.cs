using FotoApi.Features.HandleUrlTokens.CommandHandlers;
using FotoApi.Features.HandleUrlTokens.Dto;
using FotoApi.Features.HandleUrlTokens.QueryHandlers;
using FotoApi.Infrastructure.Api;
using FotoApi.Infrastructure.ExceptionsHandling;
using FotoApi.Infrastructure.Pipelines;
using FotoApi.Infrastructure.Security.Authorization;
using FotoApi.Model;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;

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
        
        group.MapGet("/roles", async Task<Results<Ok<List<RoleResponse>>, BadRequest<ErrorDetail>>> 
            (GetRolesHandler handler, FotoAppPipeline pipe, CancellationToken ct) =>
        {
            var response = await pipe.Pipe(handler.Handle, ct);
            return TypedResults.Ok(response);
        });
       
        return group;
    }
}

public class GetRolesHandler(RoleManager<Role> roleManager) : IEmptyRequestHandler<List<RoleResponse>>
{
    public async Task<List<RoleResponse>> Handle(CancellationToken cancellationToken = default)
    {
        var result = from r in roleManager.Roles
            orderby r.SortOrder
            select new RoleResponse
            {
                Name = r.Name ?? string.Empty,
            }; 
        return await result.ToListAsync(cancellationToken);
    }
}

public record RoleResponse
{
    public string Name { get; init; }
}

public record RoleRequest
{
    public string Name { get; init; }
}