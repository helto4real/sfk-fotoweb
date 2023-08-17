using FotoApi.Infrastructure.Validation.Exceptions;

namespace FotoApi.Infrastructure.Security.Authorization.Exceptions;

public class RefreshTokenNotFoundOrWrongException() : BadRequestException("Refresh token not found or wrong");