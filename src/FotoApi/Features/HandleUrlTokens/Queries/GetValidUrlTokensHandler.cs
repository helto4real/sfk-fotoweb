using FotoApi.Features.HandleUrlTokens.Model;
using FotoApi.Infrastructure.Repositories;

namespace FotoApi.Features.HandleUrlTokens.Queries;

public class GetValidUrlTokensHandler : IQueryHandler<GetValidUrlTokensQuery, List<UrlTokenResponse>>
{
    private readonly PhotoServiceDbContext _db;

    public GetValidUrlTokensHandler(PhotoServiceDbContext db)
    {
        _db = db;
    }

    public async Task<List<UrlTokenResponse>> Handle(GetValidUrlTokensQuery request,
        CancellationToken cancellationToken)
    {
        var tokens = await _db.UrlTokens
            .Where(t => t.ExpirationDate > DateTime.UtcNow)
            .Select(t => new UrlTokenResponse
            {
                Id = t.Id,
                Token = t.Token,
                UrlTokenType = t.UrlTokenType,
                ExpirationDate = t.ExpirationDate
            })
            .ToListAsync();

        return tokens;
    }
}