namespace FotoApi.Infrastructure.Validation.Exceptions;

public abstract class BadRequestException(string message) : ApiException("Bad Request", message);