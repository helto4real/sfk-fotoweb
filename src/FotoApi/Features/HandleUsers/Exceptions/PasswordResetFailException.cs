using FotoApi.Infrastructure.Validation.Exceptions;

namespace FotoApi.Features.HandleUsers.Exceptions;

public class PasswordResetFailException : BadRequestException
{
    public PasswordResetFailException(string message) : base(message)
    {
    }
}