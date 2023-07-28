using FotoApi.Features.HandleSubmissions.HandleStBilder.Dto;
using FotoApi.Infrastructure.Repositories.PhotoServiceDbContext;

namespace FotoApi.Features.HandleSubmissions.HandleStBilder.Queries;

public class GetAllStBilderHandler(PhotoServiceDbContext db) : IHandler<bool, List<StBildResponse>>
{
    private readonly StBildResponseMapper _responseMapper = new();

    public async Task<List<StBildResponse>> Handle(bool showPackagedImages, CancellationToken ct)
    {
        return showPackagedImages
            ? await db.StBilder
                .Select(e => _responseMapper.ToStBildResponse(e))
                .ToListAsync(ct)
            : await db.StBilder
                .Where(e => e.IsUsed == false).Select(e => _responseMapper.ToStBildResponse(e))
                .ToListAsync(ct);
    }
}