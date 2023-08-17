namespace FotoApi.Infrastructure.Validation.Exceptions;

public abstract class NotFoundException(string message) : ApiException("Not Found", message);