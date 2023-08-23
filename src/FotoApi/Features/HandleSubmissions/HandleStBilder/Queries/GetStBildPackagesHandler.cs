using FotoApi.Features.HandleSubmissions.HandleStBilder.Dto;
using FotoApi.Infrastructure.Repositories.PhotoServiceDbContext;

namespace FotoApi.Features.HandleSubmissions.HandleStBilder.Queries;

public class GetStBildPackagesHandler(PhotoServiceDbContext db) : IHandler<bool, IReadOnlyCollection<StBildPackageResponse>>
{
    public async Task<IReadOnlyCollection<StBildPackageResponse>> Handle(bool returnDelivered, CancellationToken cancellationToken = default)
    {
        var packagesQuery = returnDelivered switch
        {
            true => db.StPackage.OrderByDescending(p => p.CreatedDate),
            false=> db.StPackage.Where(n=>n.IsDelivered==false).OrderBy(p => p.CreatedDate)
        };
        var packages = await packagesQuery.ToListAsync(cancellationToken);
        return packages.Select(x => new StBildPackageResponse
        {
            Id = x.Id,
            IsDelivered = x.IsDelivered,
            PackageNumber = x.PackageNumber,
            UpdatedDate = x.UpdatedDate
        }).ToList();
    }
}