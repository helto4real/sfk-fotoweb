using MediatR;

namespace FotoApi.Features.HandleUsers;

public record UserDeletedNotification(string UserName) : INotification;