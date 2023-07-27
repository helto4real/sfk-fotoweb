using FotoApi.Features.HandleUrlTokens.Dto;
using FotoApi.Infrastructure.Repositories.PhotoServiceDbContext;

namespace FotoApi.Features.HandleUrlTokens.QueryHandlers;

public class GetValidUrlTokensHandler(PhotoServiceDbContext db) : IEmptyRequestHandler<List<UrlTokenResponse>>
{
    public async Task<List<UrlTokenResponse>> Handle(
        CancellationToken cancellationToken)
    {
        var tokens = await db.UrlTokens
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