using FotoApi.Features.HandleStBilder.Dto;
using FotoApi.Infrastructure.Repositories;
using StBildMapper = FotoApi.Features.HandleStBilder.Dto.StBildMapper;

namespace FotoApi.Features.HandleStBilder.Queries;

public class GetAllStBilderForCurrentUserHandler : IQueryHandler<GetAllStBilderForCurrentUserQuery, List<StBildResponse>>
{
    private readonly PhotoServiceDbContext _db;
    private readonly StBildMapper _mapper = new();
    public GetAllStBilderForCurrentUserHandler(PhotoServiceDbContext db)
    {
        _db = db;
    }
    public async Task<List<StBildResponse>> Handle(GetAllStBilderForCurrentUserQuery request, CancellationToken cancellationToken)
    {
        return request.ShowPackagedImages ? 
            await _db.StBilder
                .Where(e => e.OwnerReference == request.Owner.Id).Select(e => _mapper.ToStBildResponse(e))
                .ToListAsync(cancellationToken: cancellationToken) :
            await _db.StBilder
                .Where(e => e.OwnerReference == request.Owner.Id && e.IsUsed == false).Select(e => _mapper.ToStBildResponse(e))
                .ToListAsync(cancellationToken: cancellationToken);
    }
}