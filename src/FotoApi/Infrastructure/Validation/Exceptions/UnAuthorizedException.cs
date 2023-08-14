namespace FotoApi.Infrastructure.Validation.Exceptions;

public class UnAuthorizedException : ApiException
{
    public UnAuthorizedException(string message) : base("Unauthorized", message)
    {
    }
}