using FotoApi.Features.HandleUrlTokens.Exceptions;
using FotoApi.Infrastructure.Repositories;

namespace FotoApi.Features.HandleUrlTokens.Commands;

public class DeleteTokenFromIdHandler : ICommandHandler<DeleteTokenFromIdCommand>
{
    private readonly PhotoServiceDbContext _db;

    public DeleteTokenFromIdHandler(PhotoServiceDbContext db)
    {
        _db = db;
    }

    public async Task Handle(DeleteTokenFromIdCommand request, CancellationToken cancellationToken)
    {
        var urlToken = await _db.UrlTokens.FindAsync(request.Id);
        if (urlToken is null)
            throw new UrlTokenNotFoundException(request.Id);

        _db.UrlTokens.Remove(urlToken);
        await _db.SaveChangesAsync();
    }
}