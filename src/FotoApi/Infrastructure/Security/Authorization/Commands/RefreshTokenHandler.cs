using FotoApi.Infrastructure.Repositories;
using FotoApi.Infrastructure.Security.Authentication;
using FotoApi.Infrastructure.Security.Authorization.Dto;
using FotoApi.Infrastructure.Security.Authorization.Exceptions;
using FotoApi.Model;
using Microsoft.AspNetCore.Identity;

namespace FotoApi.Infrastructure.Security.Authorization.Commands;

public class RefreshTokenHandler : ICommandHandler<RefreshTokenCommand, UserAuthorizedResponse?>
{
    private readonly PhotoServiceDbContext _db;
    private readonly ITokenService _tokenService;
    private readonly UserManager<User> _userManager;

    public RefreshTokenHandler(PhotoServiceDbContext db, ITokenService tokenService, UserManager<User> userManager)
    {
        _db = db;
        _tokenService = tokenService;
        _userManager = userManager;
    }
    public async Task<UserAuthorizedResponse?> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.UserName == request.Username && u.RefreshToken == request.RefreshToken, cancellationToken);
        if (user is null)
        {
            throw new RefreshTokenNotFoundOrWrongException();
        }
        var (refreshToken, expireTime) = _tokenService.GenerateRefreshToken();
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpirationDate = expireTime;
        _db.Users.Update(user);
        await _db.SaveChangesAsync(cancellationToken);
        var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
        return new UserAuthorizedResponse
        {
            UserName = user.UserName!,
            IsAdmin = isAdmin,
            Email = user.Email!,
            Token = _tokenService.GenerateToken(user.UserName!, isAdmin),
            RefreshToken = refreshToken,
            RefreshTokenExpiration = expireTime
        };
    }
}