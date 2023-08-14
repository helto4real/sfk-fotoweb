using FotoApi.Infrastructure.Validation.Exceptions;

namespace FotoApi.Infrastructure.Security.Authorization.Exceptions;

public class UserNotAuthorizedException : UnAuthorizedException
{
    public UserNotAuthorizedException(string message) : base(message)
    {
    }
}