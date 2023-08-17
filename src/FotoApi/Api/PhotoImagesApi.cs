using FotoApi.Features.HandleImages.CommandHandlers;
using FotoApi.Features.HandleImages.Dto;
using FotoApi.Features.HandleImages.QueryHandlers;
using FotoApi.Infrastructure.Api;
using FotoApi.Infrastructure.ExceptionsHandling;
using FotoApi.Infrastructure.Pipelines;
using FotoApi.Infrastructure.Security.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace FotoApi.Api;

internal static class PhotoImagesApi
{
    private static int MaxAllowedImageSize => 1024 * 1024 * 100;

    public static RouteGroupBuilder MapPhotoImageApi(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/images");

        group.WithTags("PhotoImages");

        // Add security requirements, all incoming requests to this API *must*
        // be authenticated with a valid user.
        group.RequireAuthorization(pb => pb.RequireCurrentUser())
            .AddOpenApiSecurityRequirement();
        // Rate limit all of the APIs
        group.RequirePerUserRateLimit();
        // group.DisableAntiforgery();
        // Validate the parameters
        //
        group.MapGet("user",
            async Task<Results<Ok<List<ImageResponse>>, BadRequest<ErrorDetail>>>
                (GetAllImagesForUserHandler handler, FotoAppPipeline pipe, CancellationToken ct) =>
            {
                var result = await pipe.Pipe(handler.Handle, ct);
                return TypedResults.Ok(result);
            });

        group.MapGet("/{id:guid}",
            async Task<Results<Ok<ImageResponse>, BadRequest<ErrorDetail>, NotFound<ErrorDetail>>> 
                (Guid id, GetUserImageHandler handler, FotoAppPipeline pipe, CancellationToken ct) =>
            {
                var result = await pipe.Pipe(id, handler.Handle, ct);
                return TypedResults.Ok(result);
            });
        
        // Get image stream by id
        group.MapGet("image/{id:guid}",  async Task<IResult> 
                (Guid id, HttpRequest req, GetImageStreamHandler handler, FotoAppPipeline pipe, CancellationToken ct) =>
                {
                    var isThumbnail = req.Query.ContainsKey("thumb");
                    var file = await pipe.Pipe(new GetImageStreamQuery(id, isThumbnail), handler.Handle, ct);
                    return Results.Stream(file, "image/jpeg");
                })
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<Stream>(contentType: "image/jpeg");

         // group.MapGet("/antiforgerytoken")
         group.MapPost("/",
             [IgnoreAntiforgeryToken] async Task<Results<Created<ImageResponse>, BadRequest<ErrorDetail>>>
                (HttpContext ctx, IFormFile file, SaveImageFromStreamHandler handler, FotoAppPipeline pipe, CancellationToken ct) =>
            {
                // The image always needs to contain metadata
                if (!ctx.Request.Form.ContainsKey("title"))
                    return TypedResults.BadRequest(new ErrorDetail {Title = "Title is required", StatusCode = StatusCodes.Status400BadRequest});
                
                var metadataType = ctx.Request.Form.ContainsKey("metadataType")
                    ? ctx.Request.Form["metadataType"].ToString() : null;
                var metadata = ctx.Request.Form.ContainsKey("metadata")
                    ? ctx.Request.Form["metadata"].ToString()
                    : null;
                
                var title = ctx.Request.Form["title"];
    
                var result = await pipe.Pipe(new SaveImageFromStreamRequest(
                    file.OpenReadStream(), title.ToString(), metadataType, 
                    metadata, file.FileName), handler.Handle, ct);

                return TypedResults.Created($"/api/images/{result.Id}", result);
            }).DisableAntiforgery();

        group.MapPut("{id:guid}",
            async Task<Results<Ok, BadRequest<ErrorDetail>, NotFound<ErrorDetail>>> (Guid id, ImageRequest request,
                UpdateImageHandler handler, FotoAppPipeline pipe, CancellationToken ct) =>
            {
                await pipe.Pipe(new UpdateImageRequest(id, request.Title, request.FileName), handler.Handle, ct);
                return TypedResults.Ok();
            });

        group.MapDelete("{id:guid}",
            async Task<Results<Ok, BadRequest<ErrorDetail>, NotFound<ErrorDetail>>> 
                (Guid id, DeleteImageHandler handler, FotoAppPipeline pipe, CancellationToken ct) =>
            {
                await pipe.Pipe(id, handler.Handle, ct);
                return TypedResults.Ok();
            });

        return group;
    }
}