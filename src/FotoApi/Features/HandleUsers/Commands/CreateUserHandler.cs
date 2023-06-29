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

        var result = await _userManager.CreateAsync(new User { UserName = command.UserName, Email = command.Email },
            command.Password);

        if (!result.Succeeded)
            throw new UserException(result.Errors.Select(e => e.Description));

        var user = await _userManager.FindByNameAsync(command.UserName);
        if (user is null)
            throw new UserNotFoundException(command.UserName);

        // Create token for email confirmation and add data about the username
        var newToken = UrlTokenCreator.CreateUrlTokenFromUrlTokenType(UrlTokenType.ConfirmEmail);
        newToken.Data = user.Id;
        var urlToken = _db.UrlTokens.Add(newToken);
        await _db.SaveChangesAsync(cancellationToken);

        await _mediator.Publish(new UserCreatedNotification(user.Email!, urlToken.Entity.Token), cancellationToken);
        _logger.LogInformation("Skapat användare med användarnam {UserName}", command.UserName);
        return _userMapper.ToUserResponse(user, false);
    }
}