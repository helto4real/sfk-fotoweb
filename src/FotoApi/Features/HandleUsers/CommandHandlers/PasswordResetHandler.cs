using System.Text;
using FotoApi.Features.HandleUsers.Dto;
using FotoApi.Features.HandleUsers.Exceptions;
using FotoApi.Model;
using Microsoft.AspNetCore.Identity;

namespace FotoApi.Features.HandleUsers.CommandHandlers;

public class PasswordResetHandler
    (UserManager<User> userManager, ILogger<PasswordResetHandler> logger) : IHandler<PasswordResetRequest>
{
    public async Task Handle(PasswordResetRequest command, CancellationToken ct = default)
    {
        var user = await userManager.FindByEmailAsync(command.Email);
        if (user is null)
        {
            throw new UserNotFoundException(command.Email);
        }

        var tokenBytes = Convert.FromBase64String(command.Token);
        var token = Encoding.UTF8.GetString(tokenBytes);

        var result = await userManager.ResetPasswordAsync(user, token, command.NewPassword);
        if (!result.Succeeded)
        {
            logger.LogWarning("Failed to reset password for user {UserName}, error: {Error}", user.UserName,
                result.Errors);
            throw new PasswordResetFailException("Återställa lösenordet misslyckades");
        }
    }
}