using FotoApi.Features.HandleSubmissions.HandleStBilder.Dto;
using FotoApi.Features.HandleSubmissions.HandleStBilder.Exceptions;
using FotoApi.Infrastructure.Repositories.PhotoServiceDbContext;

namespace FotoApi.Features.HandleSubmissions.HandleStBilder.Commands;

public class SetPackageDeliverStatusHandler(PhotoServiceDbContext db) : IHandler<PackageStatusRequest>
{
    public async Task Handle(PackageStatusRequest request, CancellationToken ct = default)
    {
        var stPackage = await db.StPackage.FindAsync(new object?[] { request.PackageId, ct }, cancellationToken: ct);
        if (stPackage == null)
            throw new PackageNotFoundException(request.PackageId);
        stPackage.IsDelivered = request.Delivered;
        db.Update(stPackage);
        await db.SaveChangesAsync(ct);
    }
}