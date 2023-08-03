using FotoApi.Infrastructure.Security.Authentication;
using FotoApi.Infrastructure.Security.Authorization.Dto;
using FotoApi.Model;
using Microsoft.AspNetCore.Identity;

namespace FotoApi.Infrastructure.Security.Authorization.CommandHandlers;

public class LoginUserHandler(UserManager<User> userManager, ITokenService tokenService) : IHandler<LoginUserRequest, UserAuthorizedResponse>
{
    public async Task<UserAuthorizedResponse> Handle(LoginUserRequest request, CancellationToken ct)
    {
        var user = await userManager.FindByNameAsync(request.UserName);

        if (user is null || !await userManager.CheckPasswordAsync(user, request.Password))
            throw new LoginFailedException();

        var (refreshToken, expireTime) = tokenService.GenerateRefreshToken();
        
        // Now add the new token data to the user
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpirationDate = expireTime;
        await userManager.UpdateAsync(user);
        var roles = (await userManager.GetRolesAsync(user)).AsReadOnly();
        return (new UserAuthorizedResponse
        {
            UserName = request.UserName,
            FirstName = "FirstName",
            LastName = "LastName",
            Email = user.Email!,
            Token = tokenService.GenerateToken(user.UserName!, roles),
            RefreshToken = refreshToken,
            RefreshTokenExpiration = expireTime,
            Roles = roles
        });
    }
}
