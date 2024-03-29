﻿using FotoApi.Features.HandleMembers.CommandHandlers;
using FotoApi.Features.HandleMembers.Dto;
using FotoApi.Features.HandleMembers.QueryHandlers;
using FotoApi.Infrastructure.Api;
using FotoApi.Infrastructure.ExceptionsHandling;
using FotoApi.Infrastructure.Pipelines;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FotoApi.Api;

public static class MemberApi
{
    public static RouteGroupBuilder MapMemberApi(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/members");

        group.WithTags("Members");

        group.RequireAuthorization("MemberPolicy")
            .AddOpenApiSecurityRequirement().RequireAuthorization();
        
        // Rate limit all of the APIs
        group.RequirePerUserRateLimit();

        group.MapGet("/", async Task<Results<Ok<IReadOnlyCollection<MemberListItemResponse>>, BadRequest<ErrorDetail>, NotFound<ErrorDetail>>>
            (GetAllMembersHandler handler, FotoAppPipeline pipe, CancellationToken ct) =>
        {
            var response = await pipe.Pipe( handler.Handle, ct);
            return TypedResults.Ok(response);
        }).RequireAuthorization("AdminPolicy");
        
        
        group.MapPost("/", async Task<Results<Ok<MemberResponse>, BadRequest<ErrorDetail>>>
            (NewMemberRequest request, CreateMembersHandler handler, FotoAppPipeline pipe, CancellationToken ct) =>
        {
            var response = await pipe.Pipe(request, handler.Handle, ct);
            return TypedResults.Ok(response);
        }).RequireAuthorization("AdminPolicy");

        group.MapPut("/", async Task<Results<Ok<MemberResponse>, BadRequest<ErrorDetail>>>
            (MemberRequest request, UpdateMembersHandler handler, FotoAppPipeline pipe, CancellationToken ct) =>
        {
            var response = await pipe.Pipe(request, handler.Handle, ct);
            return TypedResults.Ok(response);
        });

        group.MapDelete("/{memberId:guid}", async Task<Results<Ok, BadRequest<ErrorDetail>>>
            (Guid memberId, DeleteMemberHandler handler, FotoAppPipeline pipe, CancellationToken ct) =>
        {
            await pipe.Pipe(memberId, handler.Handle, ct);
            return TypedResults.Ok();
        }).RequireAuthorization("AdminPolicy");
        
        group.MapGet("/{memberId:guid}/activate", async Task<Results<Ok, NotFound<ErrorDetail>, BadRequest<ErrorDetail>>>
            (Guid memberId, SetMemberActiveStateHandler handler, FotoAppPipeline pipe, CancellationToken ct) =>
        {
            await pipe.Pipe((memberId,true), handler.Handle, ct);
            return TypedResults.Ok();
        }).RequireAuthorization("AdminPolicy");
        
        group.MapGet("/{memberId:guid}/deactivate", async Task<Results<Ok, NotFound<ErrorDetail>, BadRequest<ErrorDetail>>>
            (Guid memberId, SetMemberActiveStateHandler handler, FotoAppPipeline pipe, CancellationToken ct) =>
        {
            await pipe.Pipe((memberId,false), handler.Handle, ct);
            return TypedResults.Ok();
        }).RequireAuthorization("AdminPolicy");
        
        group.MapGet("/{memberId:guid}", async Task<Results<Ok<MemberResponse>, NotFound<ErrorDetail>, BadRequest<ErrorDetail>>>
            (Guid memberId, GetMemberByIdHandler handler, FotoAppPipeline pipe, CancellationToken ct) =>
        {
            var result = await pipe.Pipe(memberId, handler.Handle, ct);
            return TypedResults.Ok(result);
        });
        
        return group;
    }
}
