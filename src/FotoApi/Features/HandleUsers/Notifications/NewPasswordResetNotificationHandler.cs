using FotoApi.Features.HandleUsers.Dto;
using FotoApi.Features.SendEmailNotifications;
using FotoApi.Infrastructure.Settings;
using Microsoft.Extensions.Options;

namespace FotoApi.Features.HandleUsers.Notifications;

public class NewPasswordResetNotificationHandler(IMailSender mailSender, IOptions<ApiSettings> apiSettings)
{
    public async Task Handle(NewPasswordResetNotification notification, CancellationToken ct)
    {
        await mailSender.SendPasswordResetAsync(notification.Email, notification.Token, apiSettings.Value.PhotoWebUri,
            ct);
    }
}