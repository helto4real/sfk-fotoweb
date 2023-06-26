using FotoApi.Infrastructure.Validation.Exceptions;

namespace FotoApi.Features.HandleUsers.Exceptions;

public sealed class UserException : BadRequestException
{
    public UserException(string message)
        : base(message)
    {
    }

    public UserException(IEnumerable<string> messages)
        : base(messages.Aggregate((a, b) => $"{a}\n{b}"))
    {
    }
}