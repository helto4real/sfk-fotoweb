namespace FotoApi.Infrastructure.Validation.Exceptions
{
    public abstract class BadRequestException : ApiException
    {
        protected BadRequestException(string message)
            : base("Bad Request", message)
        {
        }
    }
}