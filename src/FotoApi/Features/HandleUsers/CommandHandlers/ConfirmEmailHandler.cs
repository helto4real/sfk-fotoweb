using FotoApi.Features.HandleUrlTokens.Exceptions;
using FotoApi.Features.HandleUsers.Exceptions;
using FotoApi.Infrastructure.Repositories;
using FotoApi.Infrastructure.Repositories.PhotoServiceDbContext;
using FotoApi.Model;
using Microsoft.AspNetCore.Identity;

namespace FotoApi.Features.HandleUsers.CommandHandlers;

public class ConfirmEmailHandler(PhotoServiceDbContext db, UserManager<User> userManager) : IHandler<string>
{
    public async Task Handle(string token, CancellationToken ct)
    {
        var urlToken = await db.UrlTokens.FirstOrDefaultAsync(n => n.Token == token, cancellationToken: ct);

        if (urlToken is null)
            throw new UrlTokenNotFoundException(token);

        var userId = urlToken.Data;
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
            throw new UserNotFoundException(userId);

        user.EmailConfirmed = true;
        await userManager.UpdateAsync(user);

        db.UrlTokens.Remove(urlToken);

        await db.SaveChangesAsync(ct);
    }
}