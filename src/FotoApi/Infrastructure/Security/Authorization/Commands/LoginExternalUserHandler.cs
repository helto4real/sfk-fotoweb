using FotoApi.Abstractions.Messaging;
using FotoApi.Features.HandleUrlTokens.Exceptions;
using FotoApi.Features.HandleUsers.Exceptions;
using FotoApi.Infrastructure.Repositories;
using FotoApi.Infrastructure.Repositories.PhotoServiceDbContext;
using FotoApi.Infrastructure.Security.Authentication;
using FotoApi.Infrastructure.Security.Authorization.Dto;
using FotoApi.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FotoApi.Infrastructure.Security.Authorization.Commands;

public class LoginExternalUserHandler(UserManager<User> userManager,
        ITokenService tokenService,
        PhotoServiceDbContext db)
    : IHandler<LoginExternalUserCommand, UserAuthorizedResponse>
{
    public async Task<UserAuthorizedResponse> Handle(LoginExternalUserCommand command, CancellationToken cancellationToken)
    {
        var loginUser = await userManager.FindByLoginAsync(command.Provider, command.ProviderKey);
        
        // First check if user has already registered
        var result = IdentityResult.Success;

        if (loginUser is null)
        {
            // We only create a new user if the token is valid
            if (!await db.UrlTokens.AnyAsync(e =>
                    e.Token == command.UrlToken && e.UrlTokenType == UrlTokenType.AllowAddUser))
                throw new UrlTokenNotFoundException(command.UrlToken);

            // Check if user has been pre-registered
            loginUser = await userManager.FindByNameAsync(command.UserName);

            if (loginUser is null)
            {
                throw new UserIsNotPreRegisteredException(command.UserName);
            }

            result = await userManager.AddLoginAsync(loginUser,
                    new UserLoginInfo(command.Provider, command.ProviderKey, null));
        }
        
        if (result.Succeeded)
        {
            var (refreshToken, expireTime) = tokenService.GenerateRefreshToken();
            
            loginUser.RefreshToken = refreshToken;
            loginUser.RefreshTokenExpirationDate = expireTime;
            await userManager.UpdateAsync(loginUser);
            
            var userRoles = await userManager.GetRolesAsync(loginUser);
            var isAdmin = userRoles.Contains("Admin");
            return (new UserAuthorizedResponse
            {
                UserName = command.UserName,
                FirstName = "FirstName",
                LastName = "LastName",
                Email = loginUser.Email!,
                Token = tokenService.GenerateToken(loginUser.UserName!, isAdmin),
                RefreshToken = refreshToken,
                RefreshTokenExpiration = expireTime,
                IsAdmin = isAdmin
            });
        }
        throw new UserException(result.Errors.Select(e => e.Description));
    }
}