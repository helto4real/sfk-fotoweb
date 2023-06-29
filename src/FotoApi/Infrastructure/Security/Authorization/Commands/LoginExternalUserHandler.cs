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
        var user = await _userManager.FindByLoginAsync(command.Provider, command.ProviderKey);

        var result = IdentityResult.Success;

        if (user is null)
        {
            // We only create a new user if the token is valid
            if (!await _db.UrlTokens.AnyAsync(e =>
                    e.Token == command.UrlToken && e.UrlTokenType == UrlTokenType.AllowAddUser))
                throw new UrlTokenNotFoundException(command.UrlToken);

            user = new User { UserName = command.UserName, Email = command.UserName, EmailConfirmed = true};

            result = await _userManager.CreateAsync(user);

            if (result.Succeeded)
                result = await _userManager.AddLoginAsync(user,
                    new UserLoginInfo(command.Provider, command.ProviderKey, null));
        }

        if (result.Succeeded)
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            var isAdmin = userRoles.Contains("Admin");
            return (new UserAuthorizedResponse
            {
                UserName = command.UserName,
                FirstName = "FirstName",
                LastName = "LastName",
                Email = user.Email!,
                Token = _tokenService.GenerateToken(user.UserName!, isAdmin),
                IsAdmin = isAdmin
            });
        }
        throw new UserException(result.Errors.Select(e => e.Description));
    }
}