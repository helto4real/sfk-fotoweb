using FotoApi.Abstractions.Messaging;
using FotoApi.Infrastructure.Security.Authentication;
using FotoApi.Infrastructure.Security.Authorization.Dto;
using FotoApi.Model;
using Microsoft.AspNetCore.Identity;

namespace FotoApi.Infrastructure.Security.Authorization.Commands;

public class LoginUserHandler : ICommandHandler<LoginUserCommand, AuthorizationTokenResponse>
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly ITokenService _tokenService;

    public LoginUserHandler(UserManager<User> userManager, RoleManager<Role> roleManager, ITokenService tokenService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _tokenService = tokenService;
    }
    public async Task<AuthorizationTokenResponse> Handle(LoginUserCommand command, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByNameAsync(command.UserName);

        if (user is null || !await _userManager.CheckPasswordAsync(user, command.Password))
            throw new LoginFailedException();

        var userRoles = await _userManager.GetRolesAsync(user);
        var isAdmin = userRoles.Contains("Admin");
        return (new AuthorizationTokenResponse(_tokenService.GenerateToken(user.UserName!, isAdmin), isAdmin));
    }
}