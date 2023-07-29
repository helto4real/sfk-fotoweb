namespace FotoApi.Infrastructure.Validation.Exceptions
{
    public abstract class NotFoundException : ApiException
    {
        protected NotFoundException(string message)
            : base("Not Found", message)
        {
        }
    }
}
