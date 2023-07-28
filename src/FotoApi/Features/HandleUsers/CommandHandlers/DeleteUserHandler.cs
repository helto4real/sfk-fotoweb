using FotoApi.Features.HandleUsers.Exceptions;
using FotoApi.Features.HandleUsers.Notifications;
using FotoApi.Model;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Wolverine;

namespace FotoApi.Features.HandleUsers.CommandHandlers;

public class DeleteUserHandler(UserManager<User> userManager, IMessageBus bus) : IHandler<string>
{
    public async Task Handle(string username, CancellationToken ct)
    {
        var user = await userManager.FindByNameAsync(username);

        if (user is null) throw new UserNotFoundException(username);

        await userManager.DeleteAsync(user);
        await bus.PublishAsync(new UserDeletedNotification(username));
    }
}