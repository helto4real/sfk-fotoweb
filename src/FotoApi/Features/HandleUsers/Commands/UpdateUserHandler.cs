using FotoApi.Features.HandleUsers.Exceptions;
using FotoApi.Model;
using Microsoft.AspNetCore.Identity;

namespace FotoApi.Features.HandleUsers.Commands;

public class UpdateUserHandler : ICommandHandler<UpdateUserCommand>
{
    private readonly UserManager<User> _userManager;

    public UpdateUserHandler(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByNameAsync(request.UserName);

        if (user is null) throw new UserNotFoundException(request.UserName);
        // Todo: make user updateable here
        if (request.IsAdmin)
        {
            var roleResult = await _userManager.AddToRoleAsync(user, "Admin");
            if (!roleResult.Succeeded) throw new UserException(@"Failed to add user {request.UserName} to admin role");
        }
        else
        {
            var roleResult = await _userManager.RemoveFromRoleAsync(user, "Admin");
            if (!roleResult.Succeeded)
                throw new UserException(@"Failed to remove user {request.UserName} to admin role");
        }
    }
}