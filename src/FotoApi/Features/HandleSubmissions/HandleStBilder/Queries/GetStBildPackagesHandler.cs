using FotoApi.Features.HandleSubmissions.HandleStBilder.Dto;
using FotoApi.Infrastructure.Repositories.PhotoServiceDbContext;

namespace FotoApi.Features.HandleSubmissions.HandleStBilder.Queries;

public class GetStBildPackagesHandler(PhotoServiceDbContext db) : IHandler<bool, List<StBildPackageResponse>>
{
    public async Task<List<StBildPackageResponse>> Handle(bool returnDelivered, CancellationToken cancellationToken = default)
    {
        var packages = returnDelivered switch
        {
            true => await db.StPackage.ToListAsync(cancellationToken),
            false => await db.StPackage.Where(x => !x.IsDelivered).ToListAsync(cancellationToken)
        };

        return packages.Select(x => new StBildPackageResponse()
        {
            Id = x.Id,
            IsDelivered = x.IsDelivered,
            PackageNumber = x.PackageNumber,
            UpdatedDate = x.UpdatedDate
        }).ToList();
    }
}