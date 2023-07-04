using FotoApi.Infrastructure.Validation.Exceptions;

namespace FotoApi.Features.HandleUsers.Exceptions;

public sealed class UserAlreadyRegisteredException : BadRequestException
{
    public UserAlreadyRegisteredException(string email)
        : base($"Användare {email} är redan registrerad.")
    {
    }
}