using FotoApi.Features.HandleUsers.Dto;
using FotoApi.Features.HandleUsers.Exceptions;
using FotoApi.Infrastructure.Security.Authentication;
using FotoApi.Model;
using Microsoft.AspNetCore.Identity;

namespace FotoApi.Features.HandleUsers.Commands;

public class GetUserFromTokenHandler : IQueryHandler<GetUserFromTokenQuery, UserResponse>
{
    private readonly ITokenService _tokenService;
    private readonly UserManager<User> _userManager;
    private readonly UserMapper _userMapper = new();
    
    public GetUserFromTokenHandler(ITokenService tokenService, UserManager<User> userManager)
    {
        _tokenService = tokenService;
        _userManager = userManager;
    }
    public async Task<UserResponse> Handle(GetUserFromTokenQuery query, CancellationToken cancellationToken)
    {
        var userId = _tokenService.GetUserIdByAccessTokenAsync(query.Token);
        if (userId == null)
        {
            throw new NoUserWithTokenFoundException();
        }
        var user = await _userManager.FindByNameAsync(userId);

        if (user is null) throw new UserNotFoundException(userId);
        var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
        return _userMapper.ToUserResponse(user, isAdmin);
    }
}