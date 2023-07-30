using FotoApi.Features.HandleUsers.Dto;
using FotoApi.Model;
using Microsoft.AspNetCore.Identity;

namespace FotoApi.Features.HandleUsers.QueriyHandlers;

public class GetUsersHandler(UserManager<User> userManager) : IEmptyRequestHandler<List<UserResponse>>
{
    private readonly UserMapper _userMapper = new();
    
    public async Task<List<UserResponse>> Handle(CancellationToken cancellationToken)
    {
        var userResponseList = new List<UserResponse>();
        foreach (var user in await userManager.Users.ToListAsync(cancellationToken: cancellationToken))
        {
            var isAdmin = await userManager.IsInRoleAsync(user, "Admin");
            userResponseList.Add(_userMapper.ToUserResponse(user, isAdmin));
        }

        return userResponseList;
    }
}