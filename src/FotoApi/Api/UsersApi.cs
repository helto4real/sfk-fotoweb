using System.Security.Claims;
using FotoApi.Features.HandleUrlTokens;
using FotoApi.Features.HandleUsers.CommandHandlers;
using FotoApi.Features.HandleUsers.Dto;
using FotoApi.Features.HandleUsers.Exceptions;
using FotoApi.Features.HandleUsers.Notifications;
using FotoApi.Features.HandleUsers.QueriyHandlers;
using FotoApi.Features.SendEmailNotifications;
using FotoApi.Features.Shared.Dto;
using FotoApi.Infrastructure.ExceptionsHandling;
using FotoApi.Infrastructure.Pipelines;
using FotoApi.Infrastructure.Repositories.PhotoServiceDbContext;
using FotoApi.Infrastructure.Security.Authentication;
using FotoApi.Infrastructure.Security.Authentication.Dto;
using FotoApi.Infrastructure.Security.Authorization;
using FotoApi.Infrastructure.Security.Authorization.CommandHandlers;
using FotoApi.Infrastructure.Security.Authorization.Dto;
using FotoApi.Infrastructure.Security.Authorization.Exceptions;
using FotoApi.Infrastructure.Settings;
using FotoApi.Model;
using LanguageExt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Wolverine;
using LoginUserRequest = FotoApi.Infrastructure.Security.Authorization.Dto.LoginUserRequest;

namespace FotoApi.Api;

public static class UsersApi
{
    public static RouteGroupBuilder MapUsers(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/users");

        group.WithTags("Users");

        var authMapper = new LoginMapper();

        // Get all users
        group.MapGet("/", async Task<Results<Ok<List<UserResponse>>, BadRequest<ErrorDetail>, NotFound<ErrorDetail>>>
            (GetUsersHandler handler, FotoAppPipeline pipe, CancellationToken ct) =>
        {
            var result = await pipe.Pipe(handler.Handle, ct);
            return TypedResults.Ok(result);
        }).RequireAuthorization("AdminPolicy");

        group.MapGet("user/current",
            async Task<Results<Ok<UserResponse>, BadRequest<ErrorDetail>, NotFound<ErrorDetail>>>
                (GetUserFromUsernameHandler handler, FotoAppPipeline pipe, CurrentUser user, CancellationToken ct) =>
            {
                if (user.User?.UserName is null) throw new UserNotAuthorizedException("User not authorized");
                var result = await pipe.Pipe(user.User.UserName, handler.Handle, ct);
                return TypedResults.Ok(result);
            }).RequireAuthorization();

        // Edit user
        group.MapPut("user", async Task<Results<
                Ok, BadRequest<ErrorDetail>, NotFound<ErrorDetail>>>
            (UserRequest request, UpdateUserHandler handler, FotoAppPipeline pipe, CancellationToken ct) =>
        {
            await pipe.Pipe(request, handler.Handle, ct);
            return TypedResults.Ok();
        }).RequireAuthorization("AdminPolicy");

        group.MapPut("logininfo", async Task<Results<
            Ok, BadRequest<ErrorDetail>, NotFound<ErrorDetail>>>
        (UpdateLoginInfoRequest request, UpdateLoginInfoHandler handler, FotoAppPipeline pipe,
            CancellationToken ct) =>
        {
            await pipe.Pipe(request, handler.Handle, ct);
            return TypedResults.Ok();
        }).RequireAuthorization();

        // Precreate user by providing the email address of users that are allowed to register
        group.MapPost("precreate", async Task<Results<
                Ok<UserResponse>, BadRequest<ErrorDetail>, BadRequest<ErrorDetail>>>
            (EmailRequest request, PreCreateUserHandler handler, FotoAppPipeline pipe, CancellationToken ct) =>
        {
            var response = await pipe.Pipe(request, handler.Handle, ct);
            return TypedResults.Ok(response);
        }).RequireAuthorization("AdminPolicy");

        group.MapGet("user/{userName}",
            async Task<Results<Ok<UserResponse>, BadRequest<ErrorDetail>, NotFound<ErrorDetail>>>
                (string userName, GetUserFromUsernameHandler handler, FotoAppPipeline pipe, CancellationToken ct) =>
            {
                var response = await pipe.Pipe(userName, handler.Handle, ct);
                return TypedResults.Ok(response);
            }).RequireAuthorization("AdminPolicy");

        group.MapDelete("user/{username}", async Task<Results<Ok, BadRequest<ErrorDetail>, NotFound<ErrorDetail>>>
            (string username, DeleteUserHandler handler, FotoAppPipeline pipe, CancellationToken ct) =>
        {
            await pipe.Pipe(username, handler.Handle, ct);
            return TypedResults.Ok();
        }).RequireAuthorization("AdminPolicy");

        // Creates a new user if a valid token is provided
        group.MapPost("/", async Task<Results<Ok<UserResponse>, BadRequest<ErrorDetail>>> (
            NewUserRequest request,
            CreateUserHandler handler,
            FotoAppPipeline pipe,
            CancellationToken ct) =>
        {
            var response = await pipe.Pipe(request, handler.Handle, ct);
            return TypedResults.Ok(response);
        });

        group.MapPost("/create", async Task<Results<Ok<UserAuthorizedResponse>, BadRequest<ErrorDetail>>> (
            NewUserRequest request,
            LoginAndCreateUserHandler handler, FotoAppPipeline pipe, CancellationToken ct) =>
        {
            var response = await pipe.Pipe(authMapper.ToLoginAndCreateUserCommand(request), handler.Handle, ct);
            return TypedResults.Ok(response);
        });

        group.MapPost("/token", async Task<Results<Ok<UserAuthorizedResponse>, BadRequest<ErrorDetail>>>
            (LoginUserRequest request, LoginUserHandler handler, FotoAppPipeline pipe, CancellationToken ct) =>
        {
            var response = await pipe.Pipe(request, handler.Handle, ct);
            return TypedResults.Ok(response);
        });

        group.MapGet("/token/validate", Results<Ok, BadRequest<ErrorDetail>>
            () => TypedResults.Ok()).RequireAuthorization();

        group.MapPost("/token/refresh", async Task<Results<Ok<UserAuthorizedResponse>, BadRequest<ErrorDetail>>>
            (RefreshTokenRequest request, RefreshTokenHandler handler, FotoAppPipeline pipe, CancellationToken ct) =>
        {
            var response = await pipe.Pipe(request, handler.Handle, ct);
            return TypedResults.Ok(response);
        });

        group.MapPost("/bytoken", async Task<Results<Ok<UserResponse>, BadRequest<ErrorDetail>>>
            (TokenRequest request, GetUserFromTokenHandler handler, FotoAppPipeline pipe, CancellationToken ct) =>
        {
            var response = await pipe.Pipe(request.Token, handler.Handle, ct);
            return TypedResults.Ok(response);
        });

        group.MapPost("/token/{provider}", async Task<Results<
            Ok<UserAuthorizedResponse>,
            NotFound<ErrorDetail>,
            BadRequest<ErrorDetail>>> (
            string provider,
            LoginExternalUserRequest request,
            LoginExternalUserHandler handler,
            FotoAppPipeline pipe,
            CancellationToken ct
        ) =>
        {
            var response = await pipe.Pipe(new LoginExternalUserCommand(
                UserName: request.UserName,
                Provider: provider,
                ProviderKey: request.ProviderKey,
                UrlToken: request.UrlToken), handler.Handle, ct);
            return TypedResults.Ok(response);
        });

        return group;
    }
}

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

        if (emailUpdated)
        {
            // We send an notification to the old email address that someone has changed account information
            await bus.PublishAsync(new AccountChangedNotification(oldEmail, user.UserName!));
                        
            // We send an email that requires the user to confirm the new email address
            await bus.PublishAsync(new EmailChangedNotification(command.NewEmail!, user.Id));
        }

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
            false => await userManager.AddPasswordAsync(user, newPassword),
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

        var emailUpdated = false;
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

            var result = await userManager.ChangeEmailAsync(user, newEmail, currentPassword);
            if (!result.Succeeded) throw new UserNotAuthorizedException("Fel lösenord");
            if (!await userManager.CheckPasswordAsync(user, command.CurrentPassword))
            {
                throw new UserNotAuthorizedException("Felaktigt lösenord");
            }

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

public record AccountChangedNotification(string Email, string UserName);

public record UpdateLoginInfoResponse
{
    public bool EmailUpdated { get; init; }
    public bool PasswordUpdated { get; init; }
}

public record UpdateLoginInfoRequest
{
    public string? NewUserName { get; init; }
    public string? CurrentPassword { get; init; }
    public string? NewPassword { get; init; }
    public string? NewEmail { get; init; }
    public bool IsUserExternal { get; init; }
}

public class AccountChangedNotificationHandler(IMailSender emailSender)
{
    public async Task Handle(AccountChangedNotification notification, CancellationToken cancellationToken)
    {
        var message =
            "Dina kontouppgifter på SFK Fotowebb har ändrats. Om du inte har gjort ändringarna, kontakta administratören.";
        await emailSender.SendAccountChangedEmailAsync(notification.Email, message, cancellationToken);
    }
}
public class EmailChangedNotificationHandler(IMailSender emailSender,
    IOptions<ApiSettings> apiSettingsOptions, PhotoServiceDbContext db)
{
    public async Task Handle(EmailChangedNotification notification, CancellationToken cancellationToken)
    {
        var token = UrlTokenCreator.CreateUrlTokenFromUrlTokenType(UrlTokenType.ConfirmEmail, notification.UserId);
        await db.AddAsync(token, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        await emailSender.SendEmailConfirmationAsync(notification.Email, token.Token,
            apiSettingsOptions.Value.PhotoWebUri, cancellationToken);
    }
}

public record EmailChangedNotification(string Email,  string UserId);