using FotoApi.Features.HandleUrlTokens.Commands;
using FotoApi.Features.HandleUrlTokens.Model;
using FotoApi.Features.HandleUrlTokens.Queries;
using FotoApi.Features.HandleUsers.Commands;
using FotoApi.Features.HandleUsers.Dto;
using FotoApi.Features.HandleUsers.Queries;
using FotoApi.Infrastructure.Api;
using FotoApi.Infrastructure.ExceptionsHandling;
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
        
        var urlTokenMapper = new UrlTokenMapper();
        var userMapper = new UserMapper();

        // Get all users
        group.MapGet("users", async Task<Results<Ok<List<UserResponse>>, NotFound<ErrorDetail>>> (IMediator mediator) =>
        {
            var result = await mediator.Send(new GetUsersQuery());
            return TypedResults.Ok(result);
        }); 
        
        // Edit user
        group.MapPut("user", async Task<Results<
            Ok, BadRequest<ErrorDetail>, NotFound<ErrorDetail>>> (UserRequest request, IMediator mediator) =>
        {
            await mediator.Send(userMapper.ToEditUserCommand(request));
            return TypedResults.Ok();
        });
        
        group.MapGet("user/{userName}", async Task<Results<Ok<UserResponse>, BadRequest>> (string userName, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetUserFromUsernameQuery(userName));
            
            if (result is null)
            {
                return TypedResults.BadRequest();
            }

            return TypedResults.Ok(result);
        });
        
        group.MapDelete("user/{username}", async Task<Results<Ok, NotFound<ErrorDetail>>> (string username, IMediator mediator) =>
        {
            await mediator.Send(new DeleteUserCommand(username));
            return TypedResults.Ok();
        });
        
        group.MapPost("token/addtokenbytype", async Task<Results<Ok< UrlTokenResponse>, BadRequest<ErrorDetail>>> (UrlTokenTypeRequest request, IMediator mediator) =>
        {
            var command = urlTokenMapper.ToAddUrlTokenFromUrlTokenTypeCommand(request);
            var result = await mediator.Send(command);
            
            return TypedResults.Ok(result);
        });
        
        group.MapDelete("token/{id}", async Task<Results<Ok, NotFound<ErrorDetail>>> (Guid id, IMediator mediator) =>
        {
            await mediator.Send(new DeleteTokenFromIdCommand(id));
            return TypedResults.Ok();
        });
        
        group.MapGet("token/{token}", async Task<Results<Ok<UrlTokenResponse>, NotFound<ErrorDetail>>> (string token, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetUrlTokenByTokenStringQuery(token));
            
            return TypedResults.Ok(result);
        });
        
        group.MapGet("token/valid-tokens", async Task<Ok<List<UrlTokenResponse>>> (IMediator mediatr) =>
        {
            var result = await mediatr.Send(new GetValidUrlTokensQuery());
            return TypedResults.Ok(result);
        });
        return group;
    }
}