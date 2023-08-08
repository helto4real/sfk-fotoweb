using System.Text;
using FotoApi.Features.HandleUsers.Dto;
using FotoApi.Model;
using Microsoft.AspNetCore.Identity;
using Wolverine;

namespace FotoApi.Features.HandleUsers.CommandHandlers;

public class RequestPasswordResetHandler(UserManager<User> userManager, ILogger<RequestPasswordResetHandler> logger,
    IMessageBus bus) : IHandler<RequestPasswordResetRequest>
{
    public async Task Handle(RequestPasswordResetRequest command, CancellationToken ct = default)
    {
        // We log warnings to be able to track down hacker attacks
        if (string.IsNullOrEmpty(command.Email))
        {
            logger.LogWarning("Password reset with empty e-mail");
            return;
        }

        var user = await userManager.FindByEmailAsync(command.Email);
        if (user is null)
        {
            logger.LogWarning("Password reset for non-existing user using e-mail {Email}", command.Email);
            return;
        }

        if (user.Email is null)
        {
            logger.LogWarning("Password reset for user {UserName} without e-mail", user.UserName);
            return;
        }

        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var baseToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(token));

        await bus.PublishAsync(new NewPasswordResetNotification(user.Email, baseToken));
    }
}