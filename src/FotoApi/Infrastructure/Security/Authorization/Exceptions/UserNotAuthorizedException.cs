using FotoApi.Infrastructure.Validation.Exceptions;

namespace FotoApi.Infrastructure.Security.Authorization.Exceptions;

public class UserNotAuthorizedException(string message) : UnAuthorizedException(message);