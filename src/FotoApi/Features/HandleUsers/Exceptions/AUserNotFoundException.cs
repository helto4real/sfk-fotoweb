using FotoApi.Infrastructure.Validation.Exceptions;

namespace FotoApi.Features.HandleUsers.Exceptions;

public sealed class NoUserWithTokenFoundException : NotFoundException
{
    public NoUserWithTokenFoundException()
        : base($"The provider token is not valid!")
    {
    }
}