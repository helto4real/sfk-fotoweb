using FotoApi.Features.HandleImages.Dto;
using FotoApi.Features.HandleImages.Exceptions;
using FotoApi.Infrastructure.Repositories;
using FotoApi.Infrastructure.Repositories.PhotoServiceDbContext;
using FotoApi.Infrastructure.Security.Authorization;
using Wolverine;

namespace FotoApi.Features.HandleImages.CommandHandlers;

public class DeleteImageHandler(PhotoServiceDbContext db, IPhotoStore photoStore, CurrentUser currentUser,
        IMessageBus bus)
    : IHandler<Guid>
{
    public async Task Handle(Guid id, CancellationToken ct)
    {
        var imageInfo = await db.Images.FindAsync(new object?[] { id }, cancellationToken: ct);
        if (imageInfo == null)
            throw new ImageNotFoundException(id);

        var rowsAffected = await db.Images
            .Where(t => t.Id == id && (t.OwnerReference == currentUser.Id || currentUser.IsAdmin))
            .ExecuteDeleteAsync(cancellationToken: ct);

        if (rowsAffected > 0)
        {
            photoStore.DeletePhoto(imageInfo.LocalFilePath);
            await bus.PublishAsync(new ImageDeletedNotification(id));
        }
        else
            throw new ImageNotFoundException(id);
    }
}