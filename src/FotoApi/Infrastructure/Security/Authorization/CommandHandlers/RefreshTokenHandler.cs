﻿using FotoApi.Infrastructure.Repositories.PhotoServiceDbContext;
using FotoApi.Infrastructure.Security.Authentication;
using FotoApi.Infrastructure.Security.Authentication.Dto;
using FotoApi.Infrastructure.Security.Authorization.Dto;
using FotoApi.Infrastructure.Security.Authorization.Exceptions;
using FotoApi.Model;
using Microsoft.AspNetCore.Identity;

namespace FotoApi.Infrastructure.Security.Authorization.CommandHandlers;

public class RefreshTokenHandler(PhotoServiceDbContext db, ITokenService tokenService, UserManager<User> userManager, ILogger<RefreshTokenHandler> logger)
    : IHandler<RefreshTokenRequest, UserAuthorizedResponse?>
{
    public async Task<UserAuthorizedResponse?> Handle(RefreshTokenRequest request, CancellationToken ct)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.UserName == request.UserName && u.RefreshToken == request.RefreshToken, ct);
        if (user is null)
        {
            logger.LogDebug("Refresh token not found or wrong for user {User}", request.UserName);
            throw new RefreshTokenNotFoundOrWrongException();
        }
        var (refreshToken, expireTime) = tokenService.GenerateRefreshToken();
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpirationDate = expireTime;
        db.Users.Update(user);
        await db.SaveChangesAsync(ct);

        logger.LogDebug("Refreshed accesstoken from refresh token for {User}", user.UserName);
        
        var roles = userManager.GetRolesAsync(user).Result.AsReadOnly();
        return new UserAuthorizedResponse
        {
            UserName = user.UserName!,
            Roles = roles,
            Email = user.Email!,
            Token = tokenService.GenerateToken(user.UserName!, roles),
            RefreshToken = refreshToken,
            RefreshTokenExpiration = expireTime
        };
    }
}