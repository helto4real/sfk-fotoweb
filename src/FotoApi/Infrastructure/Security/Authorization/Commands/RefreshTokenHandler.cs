using FotoApi.Infrastructure.Repositories;
using FotoApi.Infrastructure.Repositories.PhotoServiceDbContext;
using FotoApi.Infrastructure.Security.Authentication;
using FotoApi.Infrastructure.Security.Authentication.Dto;
using FotoApi.Infrastructure.Security.Authorization.Dto;
using FotoApi.Infrastructure.Security.Authorization.Exceptions;
using FotoApi.Model;
using Microsoft.AspNetCore.Identity;

namespace FotoApi.Infrastructure.Security.Authorization.Commands;

public class RefreshTokenHandler(PhotoServiceDbContext db, ITokenService tokenService, UserManager<User> userManager)
    : IHandler<RefreshTokenRequest, UserAuthorizedResponse?>
{
    public async Task<UserAuthorizedResponse?> Handle(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.UserName == request.UserName && u.RefreshToken == request.RefreshToken, cancellationToken);
        if (user is null)
        {
            throw new RefreshTokenNotFoundOrWrongException();
        }
        var (refreshToken, expireTime) = tokenService.GenerateRefreshToken();
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpirationDate = expireTime;
        db.Users.Update(user);
        await db.SaveChangesAsync(cancellationToken);
        var isAdmin = await userManager.IsInRoleAsync(user, "Admin");
        return new UserAuthorizedResponse
        {
            UserName = user.UserName!,
            IsAdmin = isAdmin,
            Email = user.Email!,
            Token = tokenService.GenerateToken(user.UserName!, isAdmin),
            RefreshToken = refreshToken,
            RefreshTokenExpiration = expireTime
        };
    }
}