using FotoApi.Features.HandleImages.Dto;
using FotoApi.Infrastructure.Repositories;
using MediatR;

namespace FotoApi.Features.HandleStBilder.Notifications;

public class ImageDeletedNotificationHandler : INotificationHandler<ImageDeletedNotification>
{
    private readonly PhotoServiceDbContext _db;
    private readonly ILogger<ImageDeletedNotificationHandler> _logger;

    public ImageDeletedNotificationHandler(PhotoServiceDbContext db, ILogger<ImageDeletedNotificationHandler> logger)
    {
        _db = db;
        _logger = logger;
    }
    public async Task Handle(ImageDeletedNotification notification, CancellationToken cancellationToken)
    {
        var stBild = await _db.StBilder.Where(e => e.ImageReference == notification.Id).SingleOrDefaultAsync(cancellationToken);
        
        if (stBild is not null)
        {
            _logger.LogInformation("Removing StBild {Id} cause corresponding image was deleted", stBild.Id);
            _db.StBilder.Remove(stBild);
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}