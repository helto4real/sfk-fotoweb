using FotoApi.Features.HandleImages.Exceptions;
using FotoApi.Infrastructure.Repositories;
using FotoApi.Infrastructure.Security.Authorization;
using FotoApi.Infrastructure.Validation.Exceptions;

namespace FotoApi.Features.HandleImages.Queries;

public class GetImageStreamHandler : IQueryHandler<GetImageStreamQuery, FileStream>
{
    private readonly PhotoServiceDbContext _db;
    private readonly CurrentUser _owner;
    private readonly IPhotoStore _photoStore;

    public GetImageStreamHandler(PhotoServiceDbContext db, CurrentUser owner, IPhotoStore photoStore)
    {
        _db = db;
        _owner = owner;
        _photoStore = photoStore;
    }

    public async Task<FileStream> Handle(GetImageStreamQuery query, CancellationToken cancellationToken)
    {
        if (_owner.User is null)
            throw new ForbiddenException("You must be logged in to view images");

        var imageInfo = await _db.Images.FindAsync(query.Id);

        if (imageInfo == null)
            throw new ImageNotFoundException(query.Id);

        var file = !query.IsThumbnail
            ? _photoStore.GetStreamFromRelativePath(imageInfo.LocalFilePath)
            : _photoStore.GetThumbnailStreamFromRelativePath(imageInfo.LocalFilePath);

        if (file == null)
            throw new ImageNotFoundException(query.Id);
        return file;
    }
}