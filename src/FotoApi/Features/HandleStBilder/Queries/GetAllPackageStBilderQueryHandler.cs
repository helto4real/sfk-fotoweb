using FotoApi.Features.HandleStBilder.Dto;
using FotoApi.Infrastructure.Repositories;

namespace FotoApi.Features.HandleStBilder.Queries;

public class GetAllPackageStBilderQueryHandler : IQueryHandler<GetAllPackageStBilderQuery, List<StBildResponse>>
{
    private readonly PhotoServiceDbContext _db;
    private readonly StBildMapper _mapper = new();
    
    public GetAllPackageStBilderQueryHandler(PhotoServiceDbContext db)
    {
        _db = db;
    }
    public async Task<List<StBildResponse>> Handle(GetAllPackageStBilderQuery request, CancellationToken cancellationToken)
    {
        return await _db.StBilder.Where(b => b.IsAccepted && !b.IsUsed)
            .Select(e => _mapper.ToStBildResponse(e)).ToListAsync(cancellationToken: cancellationToken);
    }
}