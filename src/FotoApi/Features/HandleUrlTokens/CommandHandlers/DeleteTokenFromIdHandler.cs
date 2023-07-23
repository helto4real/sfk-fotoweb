using FotoApi.Features.HandleUrlTokens.Exceptions;
using FotoApi.Infrastructure.Repositories;

namespace FotoApi.Features.HandleUrlTokens.CommandHandlers;

public class DeleteTokenFromIdHandler(PhotoServiceDbContext db) : IHandler<Guid>
{
    public async Task Handle(Guid id, CancellationToken cancellationToken)
    {
        var urlToken = await db.UrlTokens.FindAsync(id);
        if (urlToken is null)
            throw new UrlTokenNotFoundException(id);

        db.UrlTokens.Remove(urlToken);
        await db.SaveChangesAsync(cancellationToken);
    }
}