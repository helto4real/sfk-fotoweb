using FotoApi.Features.HandleUrlTokens.Dto;
using FotoApi.Features.HandleUrlTokens.Exceptions;
using FotoApi.Infrastructure.Repositories.PhotoServiceDbContext;
using UrlTokenMapper = FotoApi.Features.HandleUrlTokens.Dto.UrlTokenMapper;

namespace FotoApi.Features.HandleUrlTokens.QueryHandlers;

public class GetUrlTokenByTokenStringHandler : IHandler<string, UrlTokenResponse>
{
    private readonly PhotoServiceDbContext _db;
    private readonly UrlTokenMapper _mapper = new();

    public GetUrlTokenByTokenStringHandler(PhotoServiceDbContext db)
    {
        _db = db;
    }

    public async Task<UrlTokenResponse> Handle(string token, CancellationToken cancellationToken)
    {
        var urlToken = await _db.UrlTokens.FirstOrDefaultAsync(e => e.Token == token, cancellationToken: cancellationToken);

        if (urlToken is null)
            throw new UrlTokenNotFoundException(token);

        return _mapper.ToUrlTokenResponse(urlToken);
    }
}