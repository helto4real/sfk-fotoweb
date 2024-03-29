﻿using FotoApi.Features.HandleUrlTokens;
using FotoApi.Features.HandleUrlTokens.Exceptions;
using FotoApi.Features.HandleUsers.Dto;
using FotoApi.Features.HandleUsers.Exceptions;
using FotoApi.Features.HandleUsers.Notifications;
using FotoApi.Infrastructure.Repositories.PhotoServiceDbContext;
using FotoApi.Model;
using Microsoft.AspNetCore.Identity;
using Wolverine;

namespace FotoApi.Features.HandleUsers.CommandHandlers;

public class CreateUserHandler(
        UserManager<User> userManager,
        PhotoServiceDbContext db,
        IMessageBus bus,
        ILogger<CreateUserHandler> logger)
    : IHandler<NewUserRequest, UserResponse>
{
    private readonly UserMapper _userMapper = new();

    public async Task<UserResponse> Handle(NewUserRequest request, CancellationToken ct)
    {
        if (!await db.UrlTokens.AnyAsync(e =>
                e.Token == request.UrlToken && e.UrlTokenType == UrlTokenType.AllowAddUser, cancellationToken: ct))
            throw new UrlTokenNotFoundException(request.UrlToken);

        var user = await userManager.FindByNameAsync(request.UserName);
        
        if (user is null)
            throw new UserIsNotPreRegisteredException(request.UserName);

        if (await userManager.HasPasswordAsync(user))
        {
            // User already has a password, this means the user has already registered
            throw  new UserAlreadyRegisteredException(request.UserName);
        }
        var pswdResult = await userManager.AddPasswordAsync(user, request.Password);
        if (!pswdResult.Succeeded)
        {
            throw new FailedToSetNewPasswordException(request.UserName);
        }
        // Todo: add firstname and lastname to user
        
        // Create token for email confirmation and add data about the username
        var newToken = UrlTokenCreator.CreateUrlTokenFromUrlTokenType(UrlTokenType.ConfirmEmail);
        newToken.Data = user.Id;
        var urlToken = db.UrlTokens.Add(newToken);
        await db.SaveChangesAsync(ct);

        await bus.PublishAsync(new UserCreatedNotification(user.Email!, urlToken.Entity.Token));
        logger.LogInformation("Registrerat användare med användarnam {UserName}", request.UserName);
        return _userMapper.ToUserResponse(user, Array.Empty<string>());
    }
}