using FotoApi.Infrastructure.Validation.Exceptions;

namespace FotoApi.Features.HandleUsers.Exceptions;

public sealed class UserAlreadyExistsException : BadRequestException
{
    public UserAlreadyExistsException(string email)
        : base($"Användare med e-post {email} finns redan.")
    {
    }
}