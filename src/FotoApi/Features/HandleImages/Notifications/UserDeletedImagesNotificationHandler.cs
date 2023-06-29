using FotoApi.Features.HandleUsers;
using FotoApi.Features.HandleUsers.Notifications;
using FotoApi.Infrastructure.Repositories;
using MediatR;

namespace FotoApi.Features.HandleImages.Notifications;

public class UserDeletedImagesNotificationHandler : INotificationHandler<UserDeletedNotification>
{
    private readonly PhotoServiceDbContext _db;
    private readonly IPhotoStore _photoStore;
    private readonly ILogger<UserDeletedImagesNotificationHandler> _logger;

    public UserDeletedImagesNotificationHandler(
        PhotoServiceDbContext db,
        IPhotoStore photoStore,
        ILogger<UserDeletedImagesNotificationHandler> logger)
    {
        _db = db;
        _photoStore = photoStore;
        _logger = logger;
    }

    public Task Handle(UserDeletedNotification notification, CancellationToken cancellationToken)
    {
        var userImages = _db.Images.Where(i => i.OwnerReference == notification.UserName);
        if (userImages.Any())
        {
            _logger.LogInformation($"Removing {userImages.Count()} images belonging to user {notification.UserName}");
            foreach (var userImage in userImages)
            {
                _photoStore.DeletePhoto(userImage.LocalFilePath);
            }
            _db.Images.RemoveRange(userImages);
        }
        return Task.CompletedTask;
    }
}