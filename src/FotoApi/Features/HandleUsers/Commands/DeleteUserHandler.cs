using FotoApi.Features.HandleUsers.Exceptions;
using FotoApi.Features.HandleUsers.Notifications;
using FotoApi.Model;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace FotoApi.Features.HandleUsers.Commands;

public class DeleteUserHandler : ICommandHandler<DeleteUserCommand>
{
    private readonly UserManager<User> _userManager;
    private readonly IMediator _mediator;

    public DeleteUserHandler(UserManager<User> userManager, IMediator mediator)
    {
        _userManager = userManager;
        _mediator = mediator;
    }

    public async Task Handle(DeleteUserCommand command, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByNameAsync(command.UserName);

        if (user is null) throw new UserNotFoundException(command.UserName);

        await _userManager.DeleteAsync(user);
        await _mediator.Publish(new UserDeletedNotification(command.UserName));
    }
}