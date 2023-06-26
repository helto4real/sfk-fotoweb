using FotoApi.Features.HandleStBilder.Dto;
using FotoApi.Infrastructure.Repositories;
using StBildMapper = FotoApi.Features.HandleStBilder.Dto.StBildMapper;

namespace FotoApi.Features.HandleStBilder.Queries;

public class GetAllStBilderHandler : IQueryHandler<GetAllStBilderQuery, List<StBildResponse>>
{
    private readonly PhotoServiceDbContext _db;
    private readonly StBildMapper _mapper = new();
    public GetAllStBilderHandler(PhotoServiceDbContext db)
    {
        _db = db;
    }
    public async Task<List<StBildResponse>> Handle(GetAllStBilderQuery request, CancellationToken cancellationToken)
    {
        return request.Owner.IsAdmin && !request.UseOnlyCurrentUserImages
            ? await _db.StBilder.Select(e => _mapper.ToStBildResponse(e)).ToListAsync() :
            await _db.StBilder.Where(e => e.OwnerReference == request.Owner.Id).Select(e => _mapper.ToStBildResponse(e)).ToListAsync();
    }
}