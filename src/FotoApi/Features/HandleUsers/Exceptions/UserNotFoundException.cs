using FotoApi.Infrastructure.Validation.Exceptions;

namespace FotoApi.Features.HandleUsers.Exceptions;

public sealed class UserNotFoundException : NotFoundException
{
    public UserNotFoundException(int userId)
        : base($"The user with the identifier {userId} was not found.")
    {
    }

    public UserNotFoundException(string userName)
        : base($"The user with the username {userName} was not found.")
    {
    }
}