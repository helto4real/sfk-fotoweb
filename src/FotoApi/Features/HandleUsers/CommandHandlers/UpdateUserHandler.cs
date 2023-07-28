using FotoApi.Features.HandleUsers.Dto;
using FotoApi.Features.HandleUsers.Exceptions;
using FotoApi.Model;
using Microsoft.AspNetCore.Identity;

namespace FotoApi.Features.HandleUsers.CommandHandlers;

public class UpdateUserHandler(UserManager<User> userManager) : IHandler<UserRequest>
{
    public async Task Handle(UserRequest request, CancellationToken ct)
    {
        var user = await userManager.FindByNameAsync(request.UserName);

        if (user is null) throw new UserNotFoundException(request.UserName);
        var isCurrentAdmin = await userManager.IsInRoleAsync(user, "Admin");
        // Todo: make user updateable here
        if (request.IsAdmin && !isCurrentAdmin)
        {
            var roleResult = await userManager.AddToRoleAsync(user, "Admin");
            if (!roleResult.Succeeded) throw new UserException(@"Failed to add user {request.UserName} to admin role");
        }
        else if (!request.IsAdmin && isCurrentAdmin)
        {
            var roleResult = await userManager.RemoveFromRoleAsync(user, "Admin");
            if (!roleResult.Succeeded)
                throw new UserException(@"Failed to remove user {request.UserName} to admin role");
        }
    }
}