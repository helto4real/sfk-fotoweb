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
            var roles = (await userManager.GetRolesAsync(user)).AsReadOnly();
            userResponseList.Add(_userMapper.ToUserResponse(user, roles));
        }

        return userResponseList;
    }
}