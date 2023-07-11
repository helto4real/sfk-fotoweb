using FotoApi.Features.HandleUsers.Commands;
using FotoApi.Features.HandleUsers.Dto;
using FotoApi.Features.Shared.Dto;
using FotoApi.Infrastructure.ExceptionsHandling;
using FotoApi.Infrastructure.Security.Authentication.Model;
using FotoApi.Infrastructure.Security.Authorization.Commands;
using FotoApi.Infrastructure.Security.Authorization.Dto;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using UserMapper = FotoApi.Features.HandleUsers.Dto.UserMapper;

namespace FotoApi.Api;

public static class UsersApi
{
    public static RouteGroupBuilder MapUsers(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/users");

        group.WithTags("Users");

        var userMapper = new UserMapper();
        var authMapper = new LoginMapper();
        // Creates a new user if a valid token is provided
        group.MapPost("/", async Task<Results<Ok<UserResponse>, BadRequest<ErrorDetail>>> (
            NewUserRequest request, 
            IMediator mediator) =>
        {
            var result = await mediator.Send(userMapper.ToCreateUserCommand(request));
            return TypedResults.Ok(result);
        });
        
        group.MapPost("/create", async Task<Results<Ok<UserAuthorizedResponse>, BadRequest<ErrorDetail>>> (
            NewUserRequest request, 
            IMediator mediator) =>
        {
            var result = await mediator.Send(authMapper.ToLoginAndCreateUserCommand(request));
            return TypedResults.Ok(result);
        });

        group.MapPost("/token", async Task<Results<Ok<UserAuthorizedResponse>, BadRequest<ErrorDetail>>> 
            (LoginUserRequest request, IMediator mediator) =>
        {
            var result = await mediator.Send(new LoginUserCommand(request.UserName, request.Password));
            return TypedResults.Ok(result);
        }); 
        
        group.MapPost("/token/refresh", async Task<Results<Ok<UserAuthorizedResponse>, BadRequest<ErrorDetail>>> 
            (RefreshTokenRequest request, IMediator mediator) =>
        {
            var result = await mediator.Send(new RefreshTokenCommand(request.RefreshToken, request.UserName));
            return TypedResults.Ok(result);
        });
        
        group.MapPost("/bytoken", async Task<Results<Ok<UserResponse>, BadRequest<ErrorDetail>>> 
            (TokenRequest request, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetUserFromTokenQuery(request.Token));
            return TypedResults.Ok(result);
        });

        group.MapPost("/token/{provider}", async Task<Results<
            Ok<UserAuthorizedResponse>, 
            NotFound<ErrorDetail>,
            BadRequest<ErrorDetail>>> (
            string provider,
            LoginExternalUserRequest request,
            IMediator mediator
        ) =>
        {
            var result = await mediator.Send(new LoginExternalUserCommand(
                UserName: request.UserName, 
                Provider: provider, 
                ProviderKey: request.ProviderKey, 
                UrlToken: request.UrlToken));
                
            return TypedResults.Ok(result);
        });

        return group;
    }
}