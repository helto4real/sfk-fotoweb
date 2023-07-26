using FotoApi.Features.HandleUsers;
using FotoApi.Features.HandleUsers.Notifications;
using FotoApi.Infrastructure.Repositories;
using FotoApi.Infrastructure.Repositories.PhotoServiceDbContext;
using MediatR;
using Wolverine.Attributes;

namespace FotoApi.Features.HandleStBilder.Notifications;

[Transactional]
public class UserDeletedStBilderNotificationHandler(PhotoServiceDbContext db,
    ILogger<UserDeletedStBilderNotificationHandler> logger)
{
    public Task Handle(UserDeletedNotification notification, CancellationToken cancellationToken)
    {
        var userImages = db.StBilder.Where(i => i.OwnerReference == notification.UserName);
        if (userImages.Any())
        {
            logger.LogInformation($"Removing {userImages.Count()} StBilder belonging to user {notification.UserName}");
            db.StBilder.RemoveRange(userImages);
        }
        return Task.CompletedTask;
    }
}