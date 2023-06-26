using FotoApi.Features.HandleUrlTokens.Model;
using FotoApi.Infrastructure.Repositories;

namespace FotoApi.Features.HandleUrlTokens.Commands;

public class AddUrlTokenFromUrlTokenTypeHandler : ICommandHandler<AddUrlTokenFromUrlTokenTypeCommand, UrlTokenResponse>
{
    private readonly PhotoServiceDbContext _db;

    public AddUrlTokenFromUrlTokenTypeHandler(PhotoServiceDbContext db)
    {
        _db = db;
    }

    public async Task<UrlTokenResponse> Handle(AddUrlTokenFromUrlTokenTypeCommand request,
        CancellationToken cancellationToken)
    {
        var newToken = UrlTokenCreator.CreateUrlTokenFromUrlTokenType(request.UrlTokenType);
        var urlToken = _db.UrlTokens.Add(newToken);
        await _db.SaveChangesAsync();
        return new UrlTokenResponse
        {
            Id = urlToken.Entity.Id,
            UrlTokenType = urlToken.Entity.UrlTokenType,
            Token = urlToken.Entity.Token,
            ExpirationDate = newToken.ExpirationDate
        };
    }
}