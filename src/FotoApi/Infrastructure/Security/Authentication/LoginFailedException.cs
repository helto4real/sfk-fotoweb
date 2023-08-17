using FotoApi.Infrastructure.Validation.Exceptions;

namespace FotoApi.Infrastructure.Security.Authentication;

public sealed class LoginFailedException() : UnAuthorizedException($"User entered the wrong username or password.");