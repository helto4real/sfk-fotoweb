using FotoApi.Abstractions.Messaging;
using FotoApi.Features.HandleUrlTokens.Exceptions;
using FotoApi.Features.HandleUsers.Exceptions;
using FotoApi.Infrastructure.Repositories;
using FotoApi.Infrastructure.Security.Authentication;
using FotoApi.Infrastructure.Security.Authorization.Dto;
using FotoApi.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FotoApi.Infrastructure.Security.Authorization.Commands;

public class LoginExternalUserHandler : ICommandHandler<LoginExternalUserCommand, UserAuthorizedResponse>
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly ITokenService _tokenService;
    private readonly PhotoServiceDbContext _db;

    public LoginExternalUserHandler(
        UserManager<User> userManager, 
        RoleManager<Role> roleManager, 
        ITokenService tokenService,
        PhotoServiceDbContext db)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _tokenService = tokenService;
        _db = db;
    }
    public async Task<UserAuthorizedResponse> Handle(LoginExternalUserCommand command, CancellationToken cancellationToken)
    {
        var loginUser = await _userManager.FindByLoginAsync(command.Provider, command.ProviderKey);
        
        // First check if user has already registered
        var result = IdentityResult.Success;

        if (loginUser is null)
        {
            // We only create a new user if the token is valid
            if (!await _db.UrlTokens.AnyAsync(e =>
                    e.Token == command.UrlToken && e.UrlTokenType == UrlTokenType.AllowAddUser))
                throw new UrlTokenNotFoundException(command.UrlToken);

            // Check if user has been pre-registered
            loginUser = await _userManager.FindByNameAsync(command.UserName);

            if (loginUser is null)
            {
                throw new UserIsNotPreRegisteredException(command.UserName);
            }

            result = await _userManager.AddLoginAsync(loginUser,
                    new UserLoginInfo(command.Provider, command.ProviderKey, null));
        }

        if (result.Succeeded)
        {
            var userRoles = await _userManager.GetRolesAsync(loginUser);
            var isAdmin = userRoles.Contains("Admin");
            return (new UserAuthorizedResponse
            {
                UserName = command.UserName,
                FirstName = "FirstName",
                LastName = "LastName",
                Email = loginUser.Email!,
                Token = _tokenService.GenerateToken(loginUser.UserName!, isAdmin),
                IsAdmin = isAdmin
            });
        }
        throw new UserException(result.Errors.Select(e => e.Description));
    }
}