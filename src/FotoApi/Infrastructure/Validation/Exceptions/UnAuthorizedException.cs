namespace FotoApi.Infrastructure.Validation.Exceptions;

public class UnAuthorizedException(string message) : ApiException("Unauthorized", message);