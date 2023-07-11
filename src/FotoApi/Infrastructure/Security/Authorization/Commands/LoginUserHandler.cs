using FotoApi.Abstractions.Messaging;
using FotoApi.Infrastructure.Security.Authentication;
using FotoApi.Infrastructure.Security.Authorization.Dto;
using FotoApi.Model;
using Microsoft.AspNetCore.Identity;

namespace FotoApi.Infrastructure.Security.Authorization.Commands;

public class LoginUserHandler : ICommandHandler<LoginUserCommand, UserAuthorizedResponse>
{
    private readonly UserManager<User> _userManager;
    private readonly ITokenService _tokenService;

    public LoginUserHandler(UserManager<User> userManager, ITokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }
    public async Task<UserAuthorizedResponse> Handle(LoginUserCommand command, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByNameAsync(command.UserName);

        if (user is null || !await _userManager.CheckPasswordAsync(user, command.Password))
            throw new LoginFailedException();

        var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

        var (refreshToken, expireTime) = _tokenService.GenerateRefreshToken();
        
        // Now add the new token data to the user
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpirationDate = expireTime;
        await _userManager.UpdateAsync(user);
        
        return (new UserAuthorizedResponse
        {
            UserName = command.UserName,
            FirstName = "FirstName",
            LastName = "LastName",
            Email = user.Email!,
            Token = _tokenService.GenerateToken(user.UserName!, isAdmin),
            RefreshToken = refreshToken,
            RefreshTokenExpiration = expireTime,
            IsAdmin = isAdmin
        });
    }
}

// public class LoginUserHandler : ICommandHandler<LoginUserCommand, AuthorizationTokenResponse>
// {
//     private readonly UserManager<User> _userManager;
//     private readonly RoleManager<Role> _roleManager;
//     private readonly ITokenService _tokenService;
//
//     public LoginUserHandler(UserManager<User> userManager, RoleManager<Role> roleManager, ITokenService tokenService)
//     {
//         _userManager = userManager;
//         _roleManager = roleManager;
//         _tokenService = tokenService;
//     }
//     public async Task<AuthorizationTokenResponse> Handle(LoginUserCommand command, CancellationToken cancellationToken)
//     {
//         var user = await _userManager.FindByNameAsync(command.UserName);
//
//         if (user is null || !await _userManager.CheckPasswordAsync(user, command.Password))
//             throw new LoginFailedException();
//
//         var userRoles = await _userManager.GetRolesAsync(user);
//         var isAdmin = userRoles.Contains("Admin");
//         return (new AuthorizationTokenResponse(_tokenService.GenerateToken(user.UserName!, isAdmin), isAdmin));
//     }
// }