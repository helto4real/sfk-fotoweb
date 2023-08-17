using FotoApi.Features.HandleUsers.Dto;
using FotoApi.Features.HandleUsers.Exceptions;
using FotoApi.Model;
using Microsoft.AspNetCore.Identity;

namespace FotoApi.Features.HandleUsers.QueryHandlers;

public class GetUserFromUsernameHandler(UserManager<User> userManager) : IHandler<string, UserResponse?>
{
    private readonly UserMapper _userMapper = new();

    public async Task<UserResponse?> Handle(string username, CancellationToken ct)
    {
        var user = await userManager.FindByNameAsync(username);

        if (user is null) throw new UserNotFoundException(username);
        var roles = userManager.GetRolesAsync(user).Result.AsReadOnly();
        
        return _userMapper.ToUserResponse(user, roles);
    }
}