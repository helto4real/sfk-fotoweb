using FotoApi.Features.HandleUsers.Dto;
using FotoApi.Model;
using Microsoft.AspNetCore.Identity;

namespace FotoApi.Features.HandleUsers.Queries;

public class GetUsersHandler : IQueryHandler<GetUsersQuery, List<UserResponse>>
{
    private readonly UserManager<User> _userManager;
    private readonly UserMapper _userMapper = new();

    public GetUsersHandler(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<List<UserResponse>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var userResponseList = new List<UserResponse>();
        foreach (var user in _userManager.Users)
        {
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            userResponseList.Add(_userMapper.ToUserResponse(user, isAdmin));
        }

        return userResponseList;
    }
}