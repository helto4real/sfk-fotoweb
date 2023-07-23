using FotoApi.Features.HandleStBilder.Dto;
using FotoApi.Infrastructure.Repositories;

namespace FotoApi.Features.HandleStBilder.Queries;

public class GetAllPackageStBilderQueryHandler(PhotoServiceDbContext db) : IEmptyRequestHandler<List<StBildResponse>>
{
    private readonly StBildMapper _mapper = new();

    public async Task<List<StBildResponse>> Handle(CancellationToken cancellationToken)
    {
        return await db.StBilder.Where(b => b.IsAccepted && !b.IsUsed)
            .Select(e => _mapper.ToStBildResponse(e)).ToListAsync(cancellationToken: cancellationToken);
    }
}