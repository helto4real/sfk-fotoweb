using FotoApi.Features.HandleUsers;
using FotoApi.Features.HandleUsers.Notifications;
using FotoApi.Infrastructure.Repositories;
using MediatR;

namespace FotoApi.Features.HandleStBilder.Notifications;

public class UserDeletedStBilderNotificationHandler : INotificationHandler<UserDeletedNotification>
{
    private readonly PhotoServiceDbContext _db;
    private readonly ILogger<UserDeletedStBilderNotificationHandler> _logger;

    public UserDeletedStBilderNotificationHandler(
        PhotoServiceDbContext db,
        ILogger<UserDeletedStBilderNotificationHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public Task Handle(UserDeletedNotification notification, CancellationToken cancellationToken)
    {
        var userImages = _db.StBilder.Where(i => i.OwnerReference == notification.UserName);
        if (userImages.Any())
        {
            _logger.LogInformation($"Removing {userImages.Count()} StBilder belonging to user {notification.UserName}");
            _db.StBilder.RemoveRange(userImages);
        }
        return Task.CompletedTask;
    }
}