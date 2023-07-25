using FotoApi.Features.HandleStBilder.Dto;
using FotoApi.Infrastructure.Repositories;
using FotoApi.Infrastructure.Repositories.PhotoServiceDbContext;

namespace FotoApi.Features.HandleStBilder.Queries;

public class GetAllStBilderHandler(PhotoServiceDbContext db) : IHandler<bool, List<StBildResponse>>
{
    private readonly StBildResponseMapper _responseMapper = new();

    public async Task<List<StBildResponse>> Handle(bool showPackagedImages, CancellationToken cancellationToken)
    {
        return showPackagedImages ? 
            await db.StBilder
                .Select(e => _responseMapper.ToStBildResponse(e))
                .ToListAsync(cancellationToken: cancellationToken) :
            await db.StBilder
                .Where(e=> e.IsUsed == false).Select(e => _responseMapper.ToStBildResponse(e))
                .ToListAsync(cancellationToken: cancellationToken);
    }
}