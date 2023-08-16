using FotoApi.Features.HandleImages.Dto;
using FotoApi.Infrastructure.Repositories;
using FotoApi.Infrastructure.Repositories.PhotoServiceDbContext;
using Wolverine.Attributes;

namespace FotoApi.Features.HandleImages.Notifications;

[Transactional]
public class DeleteImageCommandNotificationHandler(PhotoServiceDbContext db,
    IPhotoStore photoStore,
    ILogger<DeleteImageCommandNotificationHandler> logger)
{
    public async Task Handle(DeleteImageCommandNotification notification, CancellationToken cancellationToken)
    {
        var image = await db.Images.FindAsync(notification.ImageId);
        if (image is null)
        {
            logger.LogError("Deleting image with id {ImageId} failed. Image not found", notification.ImageId);
            return;
        }

        db.Images.Remove(image);
        await db.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Successfully deleted image with id {ImageId}", notification.ImageId);
        await db.SaveChangesAsync(cancellationToken);
        photoStore.DeletePhoto(image.LocalFilePath);
    }
}