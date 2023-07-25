using FotoApi.Features.HandleUsers.CommandHandlers;
using FotoApi.Features.HandleUsers.Dto;
using FotoApi.Features.HandleUsers.QueriyHandlers;
using FotoApi.Features.Shared.Dto;
using FotoApi.Infrastructure.ExceptionsHandling;
using FotoApi.Infrastructure.Pipelines;
using FotoApi.Infrastructure.Security.Authentication.Model;
using FotoApi.Infrastructure.Security.Authorization.Commands;
using FotoApi.Infrastructure.Security.Authorization.Dto;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using LoginUserRequest = FotoApi.Infrastructure.Security.Authorization.Commands.LoginUserRequest;
using UserMapper = FotoApi.Features.HandleUsers.Dto.UserMapper;

namespace FotoApi.Api;

public static class UsersApi
{
    public static RouteGroupBuilder MapUsers(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/users");

        group.WithTags("Users");

        var authMapper = new LoginMapper();
        // Creates a new user if a valid token is provided
        group.MapPost("/", async Task<Results<Ok<UserResponse>, BadRequest<ErrorDetail>>> (
            NewUserRequest request, 
            CreateUserHandler handler, 
            FotoAppPipeline pipe, 
            CancellationToken ct) =>
        {
            var response = await pipe.Pipe(request, handler.Handle, ct);
            return TypedResults.Ok(response);
        });
        
        group.MapPost("/create", async Task<Results<Ok<UserAuthorizedResponse>, BadRequest<ErrorDetail>>> (
            NewUserRequest request, 
            LoginAndCreateUserHandler handler, FotoAppPipeline pipe, CancellationToken ct) =>
        {
            var response = await pipe.Pipe(authMapper.ToLoginAndCreateUserCommand(request), handler.Handle, ct);
            return TypedResults.Ok(response);
        });

        group.MapPost("/token", async Task<Results<Ok<UserAuthorizedResponse>, BadRequest<ErrorDetail>>> 
            (LoginUserRequest request, LoginUserHandler handler, FotoAppPipeline pipe, CancellationToken ct) =>
        {
            var response = await pipe.Pipe(request, handler.Handle, ct);
            return TypedResults.Ok(response);
        }); 
        
        group.MapPost("/token/refresh", async Task<Results<Ok<UserAuthorizedResponse>, BadRequest<ErrorDetail>>> 
            (RefreshTokenRequest request, RefreshTokenHandler handler, FotoAppPipeline pipe, CancellationToken ct) =>
        {
            var response = await pipe.Pipe(request, handler.Handle, ct);
            return TypedResults.Ok(response);
        });
        
        group.MapPost("/bytoken", async Task<Results<Ok<UserResponse>, BadRequest<ErrorDetail>>> 
            (TokenRequest request, GetUserFromTokenHandler handler, FotoAppPipeline pipe, CancellationToken ct) =>
        {
            var response = await pipe.Pipe(request.Token, handler.Handle, ct);
            return TypedResults.Ok(response);    
        });

        group.MapPost("/token/{provider}", async Task<Results<
            Ok<UserAuthorizedResponse>, 
            NotFound<ErrorDetail>,
            BadRequest<ErrorDetail>>> (
            string provider,
            LoginExternalUserRequest request,
            LoginExternalUserHandler handler, 
            FotoAppPipeline pipe, 
            CancellationToken ct
        ) =>
        {
            var response = await pipe.Pipe(new LoginExternalUserCommand(
                UserName: request.UserName, 
                Provider: provider, 
                ProviderKey: request.ProviderKey, 
                UrlToken: request.UrlToken), handler.Handle, ct);
            return TypedResults.Ok(response);    
        });

        return group;
    }
}