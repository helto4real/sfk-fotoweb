using FotoApi.Features.HandleSubmissions.HandleStBilder.Exceptions;
using FotoApi.Infrastructure.Repositories;
using FotoApi.Infrastructure.Repositories.PhotoServiceDbContext;

namespace FotoApi.Features.HandleSubmissions.HandleStBilder.Queries;

public class GetStPackageStreamHandler(IPhotoStore photoStore, PhotoServiceDbContext db) : IHandler<Guid, Stream>
{
    public async Task<Stream> Handle(Guid id, CancellationToken ct = default)
    {
        var stPackage = await db.StPackage.FindAsync(new object?[] { id, ct }, cancellationToken: ct);
        
        if (stPackage == null)
            throw new PackageNotFoundException(id);
        
        var imagePath = photoStore.GetPackageZipFilePath(stPackage.PackageNumber);
        return new FileStream(imagePath, FileMode.Open);
    }
}