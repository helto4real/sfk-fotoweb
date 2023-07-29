using FotoApi.Infrastructure.Validation.Exceptions;

namespace FotoApi.Features.HandleMembers.Exceptions;

public class MemberException : BadRequestException
{
    public MemberException(string message) : base(message)
    {
    }
}