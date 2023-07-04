using FotoApi.Features.HandleUrlTokens;
using FotoApi.Features.HandleUrlTokens.Exceptions;
using FotoApi.Features.HandleUsers.Dto;
using FotoApi.Features.HandleUsers.Exceptions;
using FotoApi.Features.HandleUsers.Notifications;
using FotoApi.Infrastructure.Repositories;
using FotoApi.Infrastructure.Security.Authorization.Dto;
using FotoApi.Model;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace FotoApi.Features.HandleUsers.Commands;

internal class CreateUserHandler : ICommandHandler<CreateUserCommand, UserResponse>
{
    private readonly PhotoServiceDbContext _db;
    private readonly ILogger<CreateUserHandler> _logger;
    private readonly IMediator _mediator;
    private readonly UserManager<User> _userManager;
    private readonly UserMapper _userMapper = new();

    public CreateUserHandler(
        UserManager<User> userManager,
        PhotoServiceDbContext db,
        ILogger<CreateUserHandler> logger,
        IMediator mediator)
    {
        _userManager = userManager;
        _db = db;
        _logger = logger;
        _mediator = mediator;
    }

    public async Task<UserResponse> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        if (!await _db.UrlTokens.AnyAsync(e =>
                e.Token == command.UrlToken && e.UrlTokenType == UrlTokenType.AllowAddUser, cancellationToken: cancellationToken))
            throw new UrlTokenNotFoundException(command.UrlToken);

        var user = await _userManager.FindByNameAsync(command.UserName);
        
        if (user is null)
            throw new UserIsNotPreRegisteredException(command.UserName);

        if (await _userManager.HasPasswordAsync(user))
        {
            // User already has a password, this means the user has already registered
            throw  new UserAlreadyRegisteredException(command.UserName);
        }
        var pswdResult = await _userManager.AddPasswordAsync(user, command.Password);
        if (!pswdResult.Succeeded)
        {
            throw new FailedToSetNewPasswordException(command.UserName);
        }
        // Todo: add firstname and lastname to user
        
        // Create token for email confirmation and add data about the username
        var newToken = UrlTokenCreator.CreateUrlTokenFromUrlTokenType(UrlTokenType.ConfirmEmail);
        newToken.Data = user.Id;
        var urlToken = _db.UrlTokens.Add(newToken);
        await _db.SaveChangesAsync(cancellationToken);

        await _mediator.Publish(new UserCreatedNotification(user.Email!, urlToken.Entity.Token), cancellationToken);
        _logger.LogInformation("Registrerat användare med användarnam {UserName}", command.UserName);
        return _userMapper.ToUserResponse(user, false);
    }
}