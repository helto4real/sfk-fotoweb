using FotoApi.Features.HandleUsers.Dto;
using FotoApi.Features.SendEmailNotifications;

namespace FotoApi.Features.HandleUsers.Notifications;

public class AccountChangedNotificationHandler(IMailSender emailSender)
{
    public async Task Handle(AccountChangedNotification notification, CancellationToken cancellationToken)
    {
        var message =
            "Dina kontouppgifter på SFK Fotowebb har ändrats. Om du inte har gjort ändringarna, kontakta administratören.";
        await emailSender.SendAccountChangedEmailAsync(notification.Email, message, cancellationToken);
    }
}