using FotoApi.Infrastructure.Validation.Exceptions;

namespace FotoApi.Infrastructure.Security.Authorization.Exceptions;

public class RefreshTokenNotFoundOrWrongException : BadRequestException
{
    public RefreshTokenNotFoundOrWrongException() : base("Refresh tokeon not found or wrong")
    {
    }
}