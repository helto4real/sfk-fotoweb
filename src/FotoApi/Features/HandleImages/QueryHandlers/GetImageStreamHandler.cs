using FotoApi.Features.HandleImages.Exceptions;
using FotoApi.Infrastructure.Repositories;
using FotoApi.Infrastructure.Repositories.PhotoServiceDbContext;

namespace FotoApi.Features.HandleImages.QueryHandlers;

public record GetImageStreamQuery(Guid Id, bool IsThumbnail);

public class GetImageStreamHandler(PhotoServiceDbContext db, IPhotoStore photoStore)
    : IHandler<GetImageStreamQuery, FileStream>
{
    public async Task<FileStream> Handle(GetImageStreamQuery query, CancellationToken ct)
    {
        var imageInfo = await db.Images.FindAsync(new object?[] { query.Id }, cancellationToken: ct);

        if (imageInfo == null)
            throw new ImageNotFoundException(query.Id);

        var file = !query.IsThumbnail
            ? photoStore.GetStreamFromRelativePath(imageInfo.LocalFilePath)
            : photoStore.GetThumbnailStreamFromRelativePath(imageInfo.LocalFilePath);

        if (file == null)
            throw new ImageNotFoundException(query.Id);
        return file;
    }
}