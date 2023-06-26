using FotoApi.Features.HandleUsers.Dto;
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
        // Creates a new user if a valid token is provided
        group.MapPost("/", async Task<Results<Ok<UserResponse>, BadRequest<ErrorDetail>>> (
            NewUserRequest request, 
            IMediator mediator) =>
        {
            var result = await mediator.Send(userMapper.ToCreateUserCommand(request));
            return TypedResults.Ok(result);
        });

        group.MapPost("/token", async Task<Results<Ok<AuthorizationTokenResponse>, BadRequest<ErrorDetail>>> 
            (LoginUserRequest request, IMediator mediator) =>
        {
            var result = await mediator.Send(new LoginUserCommand(request.UserName, request.Password, request.IsAdmin));
            return TypedResults.Ok(result);
        });

        group.MapPost("/token/{provider}", async Task<Results<
            Ok<AuthorizationTokenResponse>, 
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