using FotoApi.Features.HandleUsers.Notifications;
using FotoApi.Infrastructure.Settings;
using MediatR;
using Microsoft.Extensions.Options;

namespace FotoApi.Features.SendEmailNotifications;

public class UserCreatedNotificationHandler : INotificationHandler<UserCreatedNotification>
{
    private readonly IMailSender _emailSender;
    private readonly IOptions<ApiSettings> _apiSettingsOptions;

    public UserCreatedNotificationHandler(IMailSender emailSender,
        IOptions<ApiSettings> apiSettingsOptions)
    {
        _emailSender = emailSender;
        _apiSettingsOptions = apiSettingsOptions;
    }
    public async Task Handle(UserCreatedNotification notification, CancellationToken cancellationToken)
    {
        await _emailSender.SendEmailConfirmationAsync(notification.Email, notification.Token,
            _apiSettingsOptions.Value.PhotoWebUri);
    }
}