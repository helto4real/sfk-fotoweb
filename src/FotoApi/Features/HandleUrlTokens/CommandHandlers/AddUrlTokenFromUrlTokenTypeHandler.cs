using FotoApi.Features.HandleUrlTokens.Dto;
using FotoApi.Infrastructure.Repositories.PhotoServiceDbContext;

namespace FotoApi.Features.HandleUrlTokens.CommandHandlers;

public class AddUrlTokenFromUrlTokenTypeHandler(PhotoServiceDbContext db) : IHandler<UrlTokenTypeRequest, UrlTokenResponse>
{
    public async Task<UrlTokenResponse> Handle(UrlTokenTypeRequest request,
        CancellationToken ct)
    {
        var newToken = UrlTokenCreator.CreateUrlTokenFromUrlTokenType(request.UrlTokenType);
        var urlToken = db.UrlTokens.Add(newToken);
        await db.SaveChangesAsync(ct);
        return new UrlTokenResponse
        {
            Id = urlToken.Entity.Id,
            UrlTokenType = urlToken.Entity.UrlTokenType,
            Token = urlToken.Entity.Token,
            ExpirationDate = newToken.ExpirationDate
        };
    }
}