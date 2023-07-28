using FotoApi.Infrastructure.Validation.Exceptions;

namespace FotoApi.Features.HandleMembers.Exceptions;

public class MemberNotFoundException : NotFoundException
{
    public MemberNotFoundException(Guid memberId) : base($"Member with id {memberId} not found.")
    {
    }
}