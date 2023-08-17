namespace FotoApi.Infrastructure.Validation.Exceptions;

public class ForbiddenException(string message) : ApiException("Forbidden", message);