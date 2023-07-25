using FotoApi.Features.HandleUsers.Exceptions;
using FotoApi.Features.HandleUsers.Notifications;
using FotoApi.Model;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace FotoApi.Features.HandleUsers.CommandHandlers;

public class DeleteUserHandler(UserManager<User> userManager, IMediator mediator) : IHandler<string>
{
    public async Task Handle(string username, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByNameAsync(username);

        if (user is null) throw new UserNotFoundException(username);

        await userManager.DeleteAsync(user);
        await mediator.Publish(new UserDeletedNotification(username));
    }
}