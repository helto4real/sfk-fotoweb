using FotoApi.Features.HandleUrlTokens;
using FotoApi.Features.HandleUrlTokens.Exceptions;
using FotoApi.Features.HandleUsers.Dto;
using FotoApi.Features.HandleUsers.Exceptions;
using FotoApi.Features.SendEmailNotifications;
using FotoApi.Infrastructure.Repositories;
using FotoApi.Infrastructure.Settings;
using FotoApi.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace FotoApi.Features.HandleUsers.Commands;

internal class CreateUserHandler : ICommandHandler<CreateUserCommand, UserResponse>
{
    private readonly IOptions<ApiSettings> _apiSettingsOptions;
    private readonly PhotoServiceDbContext _db;
    private readonly IMailSender _emailSender;
    private readonly UserManager<User> _userManager;
    private readonly UserMapper _userMapper = new();

    public CreateUserHandler(
        UserManager<User> userManager,
        IMailSender emailSender,
        IOptions<ApiSettings> apiSettingsOptions,
        PhotoServiceDbContext db)
    {
        _userManager = userManager;
        _emailSender = emailSender;
        _apiSettingsOptions = apiSettingsOptions;
        _db = db;
    }

    public async Task<UserResponse> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        if (!await _db.UrlTokens.AnyAsync(e =>
                e.Token == command.UrlToken && e.UrlTokenType == UrlTokenType.AllowAddUser))
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

        await _db.SaveChangesAsync();
        await _emailSender.SendEmailConfirmationAsync(command.Email, urlToken.Entity.Token,
            _apiSettingsOptions.Value.PhotoWebUri);

        return _userMapper.ToUserResponse(user, false);
    }
}