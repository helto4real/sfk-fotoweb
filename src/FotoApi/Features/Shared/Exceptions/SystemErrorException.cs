using FotoApi.Infrastructure.Validation.Exceptions;

namespace FotoApi.Features.Shared.Exceptions;

public class SystemErrorException : ApiException
{
    public SystemErrorException(string message): base("Systemfel", message)
    {
        
    }
}