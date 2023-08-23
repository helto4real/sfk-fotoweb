using FotoApi.Features.HandleMembers.Dto;
using FotoApi.Infrastructure.Repositories.PhotoServiceDbContext;

namespace FotoApi.Features.HandleMembers.QueryHandlers;

public class GetAllMembersHandler(PhotoServiceDbContext db) : IEmptyRequestHandler<IReadOnlyCollection<MemberListItemResponse>>
{
    public async Task<IReadOnlyCollection<MemberListItemResponse>> Handle(CancellationToken cancellationToken = default)
    {
        var members = from member in db.Members
            join user in db.Users
                on member.OwnerReference equals user.Id
            select new MemberListItemResponse
            {
                UserName = user.UserName!,
                Email = user.Email!,
                FirstName = member.FirstName,
                LastName = member.LastName,
                Id = member.Id,
                IsActive = member.IsActive
            };
        return await members.ToListAsync(cancellationToken);
    }
}