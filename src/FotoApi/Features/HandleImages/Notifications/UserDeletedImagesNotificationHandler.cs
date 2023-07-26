using FotoApi.Features.HandleImages.Dto;
using FotoApi.Features.HandleUsers;
using FotoApi.Features.HandleUsers.Notifications;
using FotoApi.Infrastructure.Repositories;
using FotoApi.Infrastructure.Repositories.PhotoServiceDbContext;
using MediatR;
using Wolverine.Attributes;

namespace FotoApi.Features.HandleImages.Notifications;

[Transactional]
public class UserDeletedImagesNotificationHandler(PhotoServiceDbContext db,
    IPhotoStore photoStore,
    ILogger<UserDeletedImagesNotificationHandler> logger)
{
    public async Task Handle(UserDeletedNotification notification, CancellationToken cancellationToken)
    {
        var userImages = db.Images.Where(i => i.OwnerReference == notification.UserName);
        if (userImages.Any())
        {
            logger.LogInformation("Removing {NrOfImages} images belonging to user {User}", userImages.Count(), notification.UserName);
            foreach (var userImage in userImages)
            {
                photoStore.DeletePhoto(userImage.LocalFilePath);
            }
            db.Images.RemoveRange(userImages);
            await db.SaveChangesAsync(cancellationToken);
        }
    }
}