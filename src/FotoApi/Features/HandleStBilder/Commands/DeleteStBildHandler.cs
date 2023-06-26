using FotoApi.Features.HandleImages.Exceptions;
using FotoApi.Infrastructure.Repositories;

namespace FotoApi.Features.HandleStBilder.Commands;

public class DeleteStBildHandler : ICommandHandler<DeleteStBildCommand>
{
    private readonly PhotoServiceDbContext _db;

    public DeleteStBildHandler(PhotoServiceDbContext db)
    {
        _db = db;
    }

    public async Task Handle(DeleteStBildCommand command, CancellationToken cancellationToken)
    {
        var imageInfo = await _db.StBilder.FindAsync(command.Id);
        if (imageInfo == null)
            throw new ImageNotFoundException(command.Id);

        var rowsAffected = await _db.StBilder
            .Where(t => t.Id == command.Id && (t.OwnerReference == command.Owner.Id || command.Owner.IsAdmin))
            .ExecuteDeleteAsync();

        if (rowsAffected == 0)
            throw new ImageNotFoundException(command.Id);

        await _db.SaveChangesAsync();
    }
}