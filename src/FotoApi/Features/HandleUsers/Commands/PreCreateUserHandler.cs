using FotoApi.Features.HandleUrlTokens.Exceptions;
using FotoApi.Features.HandleUsers.Dto;
using FotoApi.Features.HandleUsers.Exceptions;
using FotoApi.Infrastructure.Repositories;
using FotoApi.Model;
using Microsoft.AspNetCore.Identity;

namespace FotoApi.Features.HandleUsers.Commands;

public class PreCreateUserHandler : ICommandHandler<PreCreateUserCommand, UserResponse>
{
    private readonly UserManager<User> _userManager;
    private readonly PhotoServiceDbContext _db;
    private readonly ILogger<PreCreateUserHandler> _logger;
    private readonly UserMapper _userMapper = new();
    public PreCreateUserHandler(
        UserManager<User> userManager,
        PhotoServiceDbContext db,
        ILogger<PreCreateUserHandler> logger
        )
    {
        _userManager = userManager;
        _db = db;
        _logger = logger;
    }
    public async Task<UserResponse> Handle(PreCreateUserCommand request, CancellationToken cancellationToken)
    {
        // Check if user already exists, checking both to be sure
        if (await _userManager.FindByEmailAsync(request.Email) is not null)
            throw new UserAlreadyExistsException(request.Email);
        if (await _userManager.FindByNameAsync(request.Email) is not null)
            throw new UserAlreadyExistsException(request.Email);
        var result = await _userManager.CreateAsync(new User
        {
            UserName = request.Email, 
            Email = request.Email, 
            PasswordHash  = null, 
            RefreshToken = "", // No refresh token when precreating this is created on first login
            RefreshTokenExpirationDate = DateTime.MinValue
        });
        if (!result.Succeeded)
            throw new UserException(result.Errors.Select(e => e.Description));
        var user = await _userManager.FindByNameAsync(request.Email);
        if (user is null)
            throw new UserNotFoundException(request.Email);
        return _userMapper.ToUserResponse(user, false);
    }
}