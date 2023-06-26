using FotoApi.Infrastructure.Validation.Exceptions;

namespace FotoApi.Infrastructure.Security.Authentication;

public sealed class LoginFailedException : BadRequestException
{
    public LoginFailedException()
        : base($"User entered the wrong username or password.")
    {
    }
        
}