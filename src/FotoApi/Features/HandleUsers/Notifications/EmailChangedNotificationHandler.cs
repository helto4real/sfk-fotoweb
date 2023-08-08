using FotoApi.Features.HandleUrlTokens;
using FotoApi.Features.HandleUsers.Dto;
using FotoApi.Features.SendEmailNotifications;
using FotoApi.Infrastructure.Repositories.PhotoServiceDbContext;
using FotoApi.Infrastructure.Settings;
using FotoApi.Model;
using Microsoft.Extensions.Options;

namespace FotoApi.Features.HandleUsers.Notifications;

public class EmailChangedNotificationHandler(IMailSender emailSender,
    IOptions<ApiSettings> apiSettingsOptions, PhotoServiceDbContext db)
{
    public async Task Handle(EmailChangedNotification notification, CancellationToken cancellationToken)
    {
        var token = UrlTokenCreator.CreateUrlTokenFromUrlTokenType(UrlTokenType.ConfirmEmail, notification.UserId);
        await db.AddAsync(token, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        await emailSender.SendEmailConfirmationAsync(notification.Email, token.Token,
            apiSettingsOptions.Value.PhotoWebUri, cancellationToken);
    }
}