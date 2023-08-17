using FotoApi.Features.HandleSubmissions.HandleStBilder.Queries;
using FotoApi.Infrastructure.Api;
using FotoApi.Infrastructure.Pipelines;
using FotoApi.Infrastructure.Security.Authorization;

namespace FotoApi.Api;

public static class DownloadApi
{
    public static RouteGroupBuilder MapDownloadApi(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/downloads");

        group.WithTags("Downloads");

        // Add security requirements, all incoming requests to this API *must*
        // be authenticated with a valid user.
        group.RequireAuthorization(pb => pb.RequireCurrentUser())
            .AddOpenApiSecurityRequirement();

        // Rate limit all of the APIs
        group.RequirePerUserRateLimit();
        // Get image stream by id
        group.MapGet("stpackage/{id:guid}", async Task<IResult> 
                (Guid id, GetStPackageStreamHandler handler, FotoAppPipeline pipe, CancellationToken ct) =>
                {
                    var file = await pipe.Pipe(id, handler.Handle, ct);
                    return Results.Stream(file, "application/zip");
                })
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<Stream>(contentType: "application/zip")
            .RequireAuthorization("StBildAdministratorPolicy");
        return group;
    }
}