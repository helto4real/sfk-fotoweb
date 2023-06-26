using FotoApi.Features.HandleUrlTokens.Exceptions;
using FotoApi.Features.HandleUrlTokens.Model;
using FotoApi.Infrastructure.Repositories;

namespace FotoApi.Features.HandleUrlTokens.Queries;

public class GetUrlTokenByTokenStringHandler : IQueryHandler<GetUrlTokenByTokenStringQuery, UrlTokenResponse>
{
    private readonly PhotoServiceDbContext _db;
    private readonly UrlTokenMapper _mapper = new();

    public GetUrlTokenByTokenStringHandler(PhotoServiceDbContext db)
    {
        _db = db;
    }

    public async Task<UrlTokenResponse> Handle(GetUrlTokenByTokenStringQuery request,
        CancellationToken cancellationToken)
    {
        var urlToken = await _db.UrlTokens.FirstOrDefaultAsync(e => e.Token == request.TokenString);

        if (urlToken is null)
            throw new UrlTokenNotFoundException(request.TokenString);

        return _mapper.ToUrlTokenResponse(urlToken);
    }
}