using FotoApi.Infrastructure.Validation.Exceptions;

namespace FotoApi.Features.HandleUsers.Exceptions;


public sealed class UserIsNotPreRegisteredException : BadRequestException
{
    public UserIsNotPreRegisteredException(string userName)
        : base($"The user {userName} is not pre-registered.")
    {
    }
}