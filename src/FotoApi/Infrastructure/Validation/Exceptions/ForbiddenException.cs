namespace FotoApi.Infrastructure.Validation.Exceptions
{
    public class ForbiddenException : ApplicationException
    {
        public ForbiddenException(string message)
            : base("Forbidden", message)
        {
        }
    }
}
