using FotoApi.Features.HandleStBilder.Dto;
using FotoApi.Features.HandleStBilder.Queries;
using FotoApi.Features.HandleUrlTokens.CommandHandlers;
using FotoApi.Features.HandleUrlTokens.Model;
using FotoApi.Features.HandleUrlTokens.QueryHandlers;
using FotoApi.Features.HandleUsers.CommandHandlers;
using FotoApi.Features.HandleUsers.Dto;
using FotoApi.Features.HandleUsers.QueriyHandlers;
using FotoApi.Infrastructure.Api;
using FotoApi.Infrastructure.ExceptionsHandling;
using FotoApi.Infrastructure.Pipelines;
using FotoApi.Infrastructure.Security.Authorization;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using UrlTokenMapper = FotoApi.Features.HandleUrlTokens.Model.UrlTokenMapper;
using UserMapper = FotoApi.Features.HandleUsers.Dto.UserMapper;

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
        
        // Get all users
        group.MapGet("users", async Task<Results<Ok<List<UserResponse>>, NotFound<ErrorDetail>>> 
            (GetUsersHandler handler, FotoAppPipeline pipe, CancellationToken ct) =>
        {
            var result = await pipe.Pipe(handler.Handle, ct);
            return TypedResults.Ok(result);
        }); 
        
        // Edit user
        group.MapPut("user", async Task<Results<
            Ok, BadRequest<ErrorDetail>, NotFound<ErrorDetail>>> 
            (UserRequest request, UpdateUserHandler handler, FotoAppPipeline pipe, CancellationToken ct) =>
        {
            await pipe.Pipe(request, handler.Handle, ct);
            return TypedResults.Ok();
        });
        
        // Precreate user by providing the email address of users that are allowed to register
        group.MapPost("users/precreate", async Task<Results<
            Ok<UserResponse>, BadRequest<ErrorDetail>, NotFound<ErrorDetail>>> 
            (EmailRequest request, PreCreateUserHandler handler, FotoAppPipeline pipe, CancellationToken ct) =>
        {
            var response = await pipe.Pipe(request, handler.Handle, ct);
            return TypedResults.Ok(response);
        });
        
        group.MapGet("user/{userName}", async Task<Results<Ok<UserResponse>, BadRequest<ErrorDetail>, NotFound<ErrorDetail>>> 
            (string userName, GetUserFromUsernameHandler handler, FotoAppPipeline pipe, CancellationToken ct) =>
        {
            var response = await pipe.Pipe(userName, handler.Handle, ct);
            return TypedResults.Ok(response);
        });
        
        group.MapDelete("user/{username}", async Task<Results<Ok, NotFound<ErrorDetail>>> 
            (string username, DeleteUserHandler handler, FotoAppPipeline pipe, CancellationToken ct) =>
        {
            await pipe.Pipe(username, handler.Handle, ct);
            return TypedResults.Ok();
        });
        
        group.MapPost("token/addtokenbytype", async Task<Results<Ok< UrlTokenResponse>, BadRequest<ErrorDetail>>> 
            (UrlTokenTypeRequest request, AddUrlTokenFromUrlTokenTypeHandler handler, FotoAppPipeline pipe, CancellationToken ct) =>
        {
            var response = await pipe.Pipe(request, handler.Handle, ct);
            return TypedResults.Ok(response);
        });
        
        group.MapDelete("token/{id}", async Task<Results<Ok, NotFound<ErrorDetail>>> 
            (Guid id, DeleteTokenFromIdHandler handler, FotoAppPipeline pipe, CancellationToken ct) =>
        {
            await pipe.Pipe(id, handler.Handle, ct);
            return TypedResults.Ok();
        });
        
        group.MapGet("token/{token}", async Task<Results<Ok<UrlTokenResponse>, NotFound<ErrorDetail>>> 
            (string token, GetUrlTokenByTokenStringHandler handler, FotoAppPipeline pipe, CancellationToken ct) =>
        {
            var response = await pipe.Pipe(token, handler.Handle, ct);
            return TypedResults.Ok(response);
        });
        
        group.MapGet("token/valid-tokens", async Task<Ok<List<UrlTokenResponse>>> 
            (GetValidUrlTokensHandler handler, FotoAppPipeline pipe, CancellationToken ct) =>
        {
            var response = await pipe.Pipe(handler.Handle, ct);
            return TypedResults.Ok(response);
        });
        
        group.MapGet("stbilder", async Task<Results<Ok<List<StBildResponse>>, NotFound<ErrorDetail>>> 
            (bool showPackagedImages, GetAllStBilderHandler handler, FotoAppPipeline pipe, CancellationToken ct) =>
        {
            var response = await pipe.Pipe(showPackagedImages, handler.Handle, ct);
            return TypedResults.Ok(response);
        });
        return group;
    }
}