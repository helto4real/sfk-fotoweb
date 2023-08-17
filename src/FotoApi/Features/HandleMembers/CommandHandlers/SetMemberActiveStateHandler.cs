using FotoApi.Features.HandleMembers.Exceptions;
using FotoApi.Infrastructure.Repositories.PhotoServiceDbContext;

namespace FotoApi.Features.HandleMembers.CommandHandlers;

public class SetMemberActiveStateHandler(PhotoServiceDbContext db) : IHandler<(Guid, bool)>
{
    public async Task Handle((Guid, bool) request, CancellationToken ct)
    {
        var (memberId, activeState) = request;
        var member = await db.Members.FindAsync(new object?[] { memberId }, cancellationToken: ct);
        if (member is null) throw new MemberNotFoundException(memberId);
        member.IsActive = activeState;
        await db.SaveChangesAsync(ct);
    }
}