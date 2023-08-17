using FotoApi.Features.HandleMembers.Exceptions;
using FotoApi.Infrastructure.Repositories.PhotoServiceDbContext;
using FotoApi.Model;
using Microsoft.AspNetCore.Identity;

namespace FotoApi.Features.HandleMembers.CommandHandlers;

public class DeleteMemberHandler(PhotoServiceDbContext db, UserManager<User> userManager) : IHandler<Guid>
{
    public async Task Handle(Guid memberId, CancellationToken ct = default)
    {
        var member = await db.Members.FindAsync(new object?[] { memberId, ct }, cancellationToken: ct);
        if (member is null) throw new MemberNotFoundException(memberId);

        var userName = member.OwnerReference;

        db.Members.Remove(member);
        await db.SaveChangesAsync(ct);

        var user = await userManager.FindByIdAsync(userName);

        if (user is not null)
            if (await userManager.IsInRoleAsync(user, "Member"))
                await userManager.RemoveFromRoleAsync(user, "Member");
    }
}