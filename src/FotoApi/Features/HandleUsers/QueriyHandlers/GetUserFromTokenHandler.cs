using FotoApi.Features.HandleUsers.Dto;
using FotoApi.Features.HandleUsers.Exceptions;
using FotoApi.Infrastructure.Security.Authentication;
using FotoApi.Model;
using Microsoft.AspNetCore.Identity;

namespace FotoApi.Features.HandleUsers.QueriyHandlers;


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
        var isAdmin = await userManager.IsInRoleAsync(user, "Admin");
        return _userMapper.ToUserResponse(user, isAdmin);
    }
}