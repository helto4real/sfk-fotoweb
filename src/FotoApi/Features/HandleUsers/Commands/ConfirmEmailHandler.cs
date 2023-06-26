using FotoApi.Features.HandleUrlTokens.Exceptions;
using FotoApi.Features.HandleUsers.Exceptions;
using FotoApi.Infrastructure.Repositories;
using FotoApi.Model;
using Microsoft.AspNetCore.Identity;

namespace FotoApi.Features.HandleUsers.Commands;

public class ConfirmEmailHandler : ICommandHandler<ConfirmEmailCommand>
{
    private readonly PhotoServiceDbContext _db;
    private readonly UserManager<User> _userManager;

    public ConfirmEmailHandler(PhotoServiceDbContext db, UserManager<User> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async Task Handle(ConfirmEmailCommand command, CancellationToken cancellationToken)
    {
        var urlToken = await _db.UrlTokens.FirstOrDefaultAsync(n => n.Token == command.UrlToken);

        if (urlToken is null)
            throw new UrlTokenNotFoundException(command.UrlToken);

        var userId = urlToken.Data;
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            throw new UserNotFoundException(userId);

        user.EmailConfirmed = true;
        await _userManager.UpdateAsync(user);

        _db.UrlTokens.Remove(urlToken);

        await _db.SaveChangesAsync();
    }
}