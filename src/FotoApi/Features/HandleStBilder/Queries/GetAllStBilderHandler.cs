using FotoApi.Features.HandleStBilder.Dto;
using FotoApi.Infrastructure.Repositories;
using StBildMapper = FotoApi.Features.HandleStBilder.Dto.StBildMapper;

namespace FotoApi.Features.HandleStBilder.Queries;

public class GetAllStBilderHandler(PhotoServiceDbContext db) : IHandler<bool, List<StBildResponse>>
{
    private readonly StBildMapper _mapper = new();

    public async Task<List<StBildResponse>> Handle(bool showPackagedImages, CancellationToken cancellationToken)
    {
        return showPackagedImages ? 
            await db.StBilder
                .Select(e => _mapper.ToStBildResponse(e))
                .ToListAsync(cancellationToken: cancellationToken) :
            await db.StBilder
                .Where(e=> e.IsUsed == false).Select(e => _mapper.ToStBildResponse(e))
                .ToListAsync(cancellationToken: cancellationToken);
    }
}