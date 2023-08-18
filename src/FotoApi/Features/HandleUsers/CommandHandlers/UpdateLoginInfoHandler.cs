using FotoApi.Features.HandleUsers.Dto;
using FotoApi.Features.HandleUsers.Exceptions;
using FotoApi.Infrastructure.Security.Authorization;
using FotoApi.Infrastructure.Security.Authorization.Exceptions;
using FotoApi.Model;
using Microsoft.AspNetCore.Identity;
using Wolverine;

namespace FotoApi.Features.HandleUsers.CommandHandlers;

public class UpdateLoginInfoHandler(
    UserManager<User> userManager,
    IMessageBus bus,
    CurrentUser currentUser) : IHandler<UpdateLoginInfoRequest, UpdateLoginInfoResponse>
{
    public async Task<UpdateLoginInfoResponse> Handle(UpdateLoginInfoRequest command, CancellationToken ct = default)
    {
        if (currentUser.User is null)
            throw new UserNotAuthorizedException("Användare ej autentiserad");

        var userName = currentUser.User.UserName;
        if (userName is null) throw new UserNotAuthorizedException("Användare ej autentiserad");

        var isExternalProvider = command.IsUserExternal;
        var user = await userManager.FindByNameAsync(userName);

        if (user is null) throw new UserNotFoundException(userName);

        var oldEmail = user.Email!;
        var emailUpdated = await SetNewEmailIfProvided(command, userManager, isExternalProvider, user);

        var passwordUpdated = await SetNewPasswordIfProvided(command, isExternalProvider, user);

        if (!emailUpdated)
            return new UpdateLoginInfoResponse
            {
                EmailUpdated = emailUpdated,
                PasswordUpdated = passwordUpdated
            };
        
        // We send an notification to the old email address that someone has changed account information
        await bus.PublishAsync(new AccountChangedNotification(oldEmail, user.UserName!));

        // We send an email that requires the user to confirm the new email address
        await bus.PublishAsync(new EmailChangedNotification(command.NewEmail!, user.Id));

        return new UpdateLoginInfoResponse
        {
            EmailUpdated = emailUpdated,
            PasswordUpdated = passwordUpdated
        };
    }

    private async Task<bool> SetNewPasswordIfProvided(UpdateLoginInfoRequest command,
        bool isLogin, User user)
    {
        var passwordUpdated = false;

        if (string.IsNullOrWhiteSpace(command.NewPassword)) return passwordUpdated;

        if (isLogin)
        {
            throw new UserException(
                "Kan inte ändra lösenord för inloggning med externa konton. Kontakta administratören för hjälp.");
        }

        await ThrowIfEmptyOrWrongPassword(command, userManager, user);

        var newPassword = command.NewPassword.Trim();
        var hasPassword = await userManager.HasPasswordAsync(user);

        var passwordChangeResult = hasPassword switch
        {
            true => await userManager.ChangePasswordAsync(user, command.CurrentPassword ?? "", newPassword),
            false => await userManager.AddPasswordAsync(user, newPassword)
        };
        if (!passwordChangeResult.Succeeded)
            throw new UserException("Kunde inte ändra lösenordet, kontakta administratören för hjälp.");
        passwordUpdated = true;

        return passwordUpdated;
    }

    private static async Task<bool> SetNewEmailIfProvided(UpdateLoginInfoRequest command, UserManager<User> userManager,
        bool isLogin,
        User user)
    {
        if (string.IsNullOrWhiteSpace(command.NewEmail)) return false;

        bool emailUpdated;
        var newEmail = command.NewEmail.Trim();

        var existingUserByEmail = await userManager.FindByEmailAsync(command.NewEmail);
        if (existingUserByEmail is not null)
            throw new UserException(
                $"Det finns redan en användare med epostadressen {command.NewEmail}");
        if (isLogin)
        {
            await userManager.SetEmailAsync(user, command.NewEmail);
            emailUpdated = true;
        }
        else
        {
            var currentPassword = command.CurrentPassword?.Trim() ??
                                  throw new UserNotAuthorizedException("Lösenord saknas");
            if (!await userManager.CheckPasswordAsync(user, currentPassword))
            {
                throw new UserNotAuthorizedException("Felaktigt lösenord");
            }

            var result = await userManager.SetEmailAsync(user, newEmail);
            if (!result.Succeeded) throw new UserNotAuthorizedException("Fel lösenord");

            emailUpdated = true;
        }

        return emailUpdated;
    }

    private static async Task ThrowIfEmptyOrWrongPassword(UpdateLoginInfoRequest command, UserManager<User> userManager,
        User user)
    {
        var currentPassword = command.CurrentPassword?.Trim() ??
                              throw new UserNotAuthorizedException(
                                  "Du måste ange lösenord för att ändra kontouppgifter");
        if (!await userManager.CheckPasswordAsync(user, currentPassword))
            throw new UserNotAuthorizedException("Felaktigt lösenord");
    }
}