using MediatR;

namespace FotoApi.Features.HandleUsers.Notifications;

public record UserDeletedNotification(string UserName) : INotification;