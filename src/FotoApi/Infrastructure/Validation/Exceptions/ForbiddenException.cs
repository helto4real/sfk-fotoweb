namespace FotoApi.Infrastructure.Validation.Exceptions
{
    public class ForbiddenException : ApiException
    {
        public ForbiddenException(string message)
            : base("Forbidden", message)
        {
        }
    }
}
