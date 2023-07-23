using FotoApi.Features.HandleImages.Exceptions;
using FotoApi.Infrastructure.Repositories;
using FotoApi.Infrastructure.Security.Authorization;
using FotoApi.Infrastructure.Validation.Exceptions;

namespace FotoApi.Features.HandleImages.Queries;

public record GetImageStreamQuery(Guid Id, bool IsThumbnail);

public class GetImageStreamHandler(PhotoServiceDbContext db, IPhotoStore photoStore)
    : IHandler<GetImageStreamQuery, FileStream>
{
    public async Task<FileStream> Handle(GetImageStreamQuery query, CancellationToken cancellationToken)
    {
        var imageInfo = await db.Images.FindAsync(query.Id);

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