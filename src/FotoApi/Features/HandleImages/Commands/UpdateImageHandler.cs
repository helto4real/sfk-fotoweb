using FotoApi.Features.HandleImages.Exceptions;
using FotoApi.Infrastructure.Repositories;
using FotoApi.Infrastructure.Security.Authorization;

namespace FotoApi.Features.HandleImages.Commands;

public class UpdateImageHandler : ICommandHandler<UpdateImageCommand>
{
    private readonly PhotoServiceDbContext _db;
    private readonly CurrentUser _owner;
    private readonly IPhotoStore _photoStore;

    public UpdateImageHandler(PhotoServiceDbContext db, IPhotoStore photoStore, CurrentUser owner)
    {
        _db = db;
        _photoStore = photoStore;
        _owner = owner;
    }

    public async Task Handle(UpdateImageCommand command, CancellationToken cancellationToken)
    {
        var rowsAffected = await _db.Images
            .Where(t => t.Id == command.Id && (t.OwnerReference == command.Owner.Id || command.Owner.IsAdmin))
            .ExecuteUpdateAsync(updates =>
                updates.SetProperty(t => t.Title, command.Title));

        if (rowsAffected == 0)
            throw new ImageNotFoundException(command.Id);
    }
}