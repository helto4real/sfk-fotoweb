﻿using FotoApi.Features.HandleMembers.Dto;
using FotoApi.Features.HandleMembers.Exceptions;
using FotoApi.Infrastructure.Repositories.PhotoServiceDbContext;
using FotoApi.Infrastructure.Security.Authorization.Dto;

namespace FotoApi.Features.HandleMembers.QueryHandlers;

public class GetMemberByIdHandler(PhotoServiceDbContext db) : IHandler<Guid, MemberResponse>
{
    public async Task<MemberResponse> Handle(Guid memberId, CancellationToken ct = default)
    {
        if (!await db.Members.AnyAsync(x => x.Id == memberId, ct)) throw new MemberNotFoundException(memberId);

        var members = from member in db.Members
            join user in db.Users
                on member.OwnerReference equals user.Id
            where member.Id == memberId
            orderby member.FirstName
            select new MemberResponse
            {
                Id = member.Id,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                FirstName = member.FirstName,
                LastName = member.LastName,
                Address = member.Address,
                IsActive = member.IsActive,
                ZipCode = member.ZipCode,
                City = member.City,
                Roles = (from role in db.Roles
                    join userRole in db.UserRoles
                        on role.Id equals userRole.RoleId
                    where userRole.UserId == user.Id
                    select new RoleResponse {Name = role.Name!}).ToList()
            };
        return await members.SingleAsync(ct);
    }
}