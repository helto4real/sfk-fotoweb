using FotoApi.Features.HandleUsers.Notifications;
using FotoApi.Infrastructure.Settings;
using Microsoft.Extensions.Options;

namespace FotoApi.Features.SendEmailNotifications;

public class UserCreatedNotificationHandler(IMailSender emailSender,
    IOptions<ApiSettings> apiSettingsOptions)
{
    public async Task Handle(UserCreatedNotification notification, CancellationToken cancellationToken)
    {
        await emailSender.SendEmailConfirmationAsync(notification.Email, notification.Token,
            apiSettingsOptions.Value.PhotoWebUri, cancellationToken);
    }
}