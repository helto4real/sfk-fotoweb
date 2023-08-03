using FotoApi.Features.HandleUsers.Dto;
using FotoApi.Features.HandleUsers.Exceptions;
using FotoApi.Model;
using Microsoft.AspNetCore.Identity;

namespace FotoApi.Features.HandleUsers.CommandHandlers;

public class PreCreateUserHandler(UserManager<User> userManager,
        ILogger<PreCreateUserHandler> logger)
    : IHandler<EmailRequest, UserResponse>
{
    private readonly UserMapper _userMapper = new();
    private static readonly string[] noRoles = Array.Empty<string>();

    public async Task<UserResponse> Handle(EmailRequest request, CancellationToken ct)
    {
        // Check if user already exists, checking both to be sure
        if (await userManager.FindByEmailAsync(request.Email) is not null)
            throw new UserAlreadyExistsException(request.Email);
        if (await userManager.FindByNameAsync(request.Email) is not null)
            throw new UserAlreadyExistsException(request.Email);
        var result = await userManager.CreateAsync(new User
        {
            UserName = request.Email, 
            Email = request.Email, 
            PasswordHash  = null, 
            RefreshToken = "", // No refresh token when precreating this is created on first login
            RefreshTokenExpirationDate = DateTime.MinValue
        });
        if (!result.Succeeded)
            throw new UserException(result.Errors.Select(e => e.Description));
        var user = await userManager.FindByNameAsync(request.Email);
        if (user is null)
            throw new UserNotFoundException(request.Email);
        logger.LogInformation("Precreated user {Email}", request.Email);
        return _userMapper.ToUserResponse(user, noRoles);
    }
}

