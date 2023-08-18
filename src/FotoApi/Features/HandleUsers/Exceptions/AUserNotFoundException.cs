using FotoApi.Infrastructure.Validation.Exceptions;

namespace FotoApi.Features.HandleUsers.Exceptions;

public sealed class NoUserWithTokenFoundException() : NotFoundException($"The provider token is not valid!");