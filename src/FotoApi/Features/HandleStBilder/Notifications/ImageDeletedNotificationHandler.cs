using FotoApi.Features.HandleImages.Dto;
using FotoApi.Infrastructure.Repositories;
using FotoApi.Infrastructure.Repositories.PhotoServiceDbContext;
using Wolverine.Attributes;

namespace FotoApi.Features.HandleStBilder.Notifications;

public class ImageDeletedNotificationHandler(ILogger<ImageDeletedNotificationHandler> logger)
{
    [Transactional]
    public async Task Handle(ImageDeletedNotification notification, PhotoServiceDbContext db, CancellationToken cancellationToken)
    {
        var stBild = await db.StBilder.Where(e => e.ImageReference == notification.Id).SingleOrDefaultAsync(cancellationToken);
        
        if (stBild is not null)
        {
            logger.LogInformation("Removing StBild {Id} cause corresponding image was deleted", stBild.Id);
            db.StBilder.Remove(stBild);
            // No save changes needed since Wolverine will do that for us in the transaction
            // await db.SaveChangesAsync(cancellationToken);
        }
    }
}