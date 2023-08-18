using FotoApi.Features.HandleUsers.Dto;
using FotoApi.Features.HandleUsers.Exceptions;
using FotoApi.Infrastructure.Security.Authentication;
using FotoApi.Model;
using Microsoft.AspNetCore.Identity;

namespace FotoApi.Features.HandleUsers.QueryHandlers;


public class GetUserFromTokenHandler(ITokenService tokenService, UserManager<User> userManager) : IHandler<string, UserResponse>
{
    private readonly UserMapper _userMapper = new();

    public async Task<UserResponse> Handle(string token, CancellationToken ct)
    {
        var userId = tokenService.GetUserIdByAccessTokenAsync(token);
        if (userId == null)
        {
            throw new NoUserWithTokenFoundException();
        }
        var user = await userManager.FindByNameAsync(userId);

        if (user is null) throw new UserNotFoundException(userId);
        var roles = userManager.GetRolesAsync(user).Result.AsReadOnly();
        return _userMapper.ToUserResponse(user, roles);
    }
}