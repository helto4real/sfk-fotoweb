using FotoApi.Features.HandleUsers.Dto;
using FotoApi.Features.HandleUsers.Exceptions;
using FotoApi.Model;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace FotoApi.Features.HandleUsers.Queries;

public class GetUserFromUsernameHandler : IRequestHandler<GetUserFromUsernameQuery, UserResponse?>
{
    private readonly UserManager<User> _userManager;
    private readonly UserMapper _userMapper = new();

    public GetUserFromUsernameHandler(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<UserResponse?> Handle(GetUserFromUsernameQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByNameAsync(request.Username);

        if (user is null) throw new UserNotFoundException(request.Username);
        var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
        return _userMapper.ToUserResponse(user, isAdmin);
    }
}