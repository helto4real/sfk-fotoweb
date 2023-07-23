using FotoApi.Features.HandleUsers.Dto;
using FotoApi.Features.HandleUsers.Exceptions;
using FotoApi.Model;
using Microsoft.AspNetCore.Identity;

namespace FotoApi.Features.HandleUsers.QueriyHandlers;

public class GetUserFromUsernameHandler(UserManager<User> userManager) : IHandler<string, UserResponse?>
{
    private readonly UserMapper _userMapper = new();

    public async Task<UserResponse?> Handle(string username, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByNameAsync(username);

        if (user is null) throw new UserNotFoundException(username);
        var isAdmin = await userManager.IsInRoleAsync(user, "Admin");
        
        return _userMapper.ToUserResponse(user, isAdmin);
    }
}