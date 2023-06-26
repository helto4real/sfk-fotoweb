using FotoApi.Features.HandleImages.Commands;
using FotoApi.Features.HandleImages.Dto;
using FotoApi.Features.HandleImages.Queries;
using FotoApi.Infrastructure.Api;
using FotoApi.Infrastructure.ExceptionsHandling;
using FotoApi.Infrastructure.Security.Authorization;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Transforms;

namespace FotoApi;

internal static class PhotoImagesApi
{
    private static int MaxAllowedImageSize => 1024 * 1024 * 100;

    public static RouteGroupBuilder MapPhotoImages(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/images");

        group.WithTags("PhotoImages");

        // Add security requirements, all incoming requests to this API *must*
        // be authenticated with a valid user.
        group.RequireAuthorization(pb => pb.RequireCurrentUser())
            .AddOpenApiSecurityRequirement();

        // Rate limit all of the APIs
        group.RequirePerUserRateLimit();

        // Validate the parameters

        group.MapGet("user",
            async Task<Ok<List<ImageResponse>>>(CurrentUser owner, IMediator mediator) =>
            {
                var result = await mediator.Send(new GetAllImagesForUserQuery(owner));
                return TypedResults.Ok(result);
            });

        group.MapGet("/{id}",
            async Task<Results<Ok<ImageResponse>, NotFound<ErrorDetail>>> 
                (Guid id, CurrentUser owner, IMediator mediator) =>
            {
                var result = await mediator.Send(new GetUserImageQuery(id, owner));
                return TypedResults.Ok(result);
            });
        
        // Get image stream by id
        group.MapGet("/image/{id}",
                async Task<IResult> (Guid id, CurrentUser owner, HttpRequest req, IMediator mediator) =>
                {
                    var isThumbnail = req.Query.ContainsKey("thumb");

                    var file = await mediator.Send(new GetImageStreamQuery(id, owner, isThumbnail));
                    
                    return Results.Stream(file, "image/jpeg");
                })
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<Stream>(contentType: "image/jpeg");

        group.MapPost("/",
            async Task<Results<Created<ImageResponse>, BadRequest<ErrorDetail>>>
                (HttpContext ctx, IFormFile file, CurrentUser owner, IMediator mediator) =>
            {
                // The image always needs to contain metadata
                if (!ctx.Request.Form.ContainsKey("title"))
                    return TypedResults.BadRequest(new ErrorDetail(){Title = "Title is required", StatusCode = StatusCodes.Status400BadRequest});
                
                string? metadataType = ctx.Request.Form.ContainsKey("metadataType")
                    ? ctx.Request.Form["metadataType"].ToString() : null;
                string? metadata = ctx.Request.Form.ContainsKey("metadata")
                    ? ctx.Request.Form["metadata"].ToString()
                    : null;
                
                var title = ctx.Request.Form["title"];
    
                var result = await mediator.Send(new SaveImageFromStreamCommand(
                    file.OpenReadStream(), title.ToString(), metadataType, 
                    metadata, file.FileName, owner));

                return TypedResults.Created($"/image/{result.Id}", result);
            });

        group.MapPut("/{id}",
            async Task<Results<Ok, NotFound<ErrorDetail>>> (Guid id, ImageRequest request,
                CurrentUser owner, IMediator mediator) =>
            {
                await mediator.Send(new UpdateImageCommand(id, request.Title, request.FileName, owner));
                return TypedResults.Ok();
            });

        group.MapDelete("/{id}",
            async Task<Results<Ok, NotFound<ErrorDetail>>> 
                (Guid id, CurrentUser owner, IMediator mediator) =>
            {
                await mediator.Send(new DeleteImageCommand(id, owner));
                return TypedResults.Ok();
            });

        return group;
    }
}