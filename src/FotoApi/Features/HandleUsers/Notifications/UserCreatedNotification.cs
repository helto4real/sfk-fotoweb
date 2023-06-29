using MediatR;

namespace FotoApi.Features.HandleUsers.Notifications;

public record UserCreatedNotification(string Email, string Token) : INotification;
