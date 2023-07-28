using FotoApi.Features.HandleMembers.Dto;
using FotoApi.Features.HandleUsers.Exceptions;
using FotoApi.Infrastructure.Repositories.PhotoServiceDbContext;
using FotoApi.Model;
using Microsoft.AspNetCore.Identity;

namespace FotoApi.Features.HandleMembers.CommandHandlers;

public class CreateMembersHandler
    (PhotoServiceDbContext db, UserManager<User> userManager) : IHandler<NewMemberRequest, MemberResponse>
{
    public async Task<MemberResponse> Handle(NewMemberRequest request, CancellationToken ct = default)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            user = new User
            {
                UserName = request.Email,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                RefreshToken = "",
                RefreshTokenExpirationDate = DateTime.MinValue
            };
            var result = await userManager.CreateAsync(user);
            if (!result.Succeeded) throw new UserException(result.Errors.Select(e => e.Description));
        }
        else
        {
            if (db.Members.Any(m => m.OwnerReference == user.Id))
                throw new UserException("Member with that email already exists");
        }

        if (user.PhoneNumber != request.PhoneNumber)
        {
            user.PhoneNumber = request.PhoneNumber;
            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded) throw new UserException(result.Errors.Select(e => e.Description));
        }

        if (!await userManager.IsInRoleAsync(user, "Member"))
        {
            var result = await userManager.AddToRoleAsync(user, "Member");
            if (!result.Succeeded) throw new UserException(result.Errors.Select(e => e.Description));
        }

        var member = new Member
        {
            OwnerReference = user.Id,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Address = request.Address,
            ZipCode = request.ZipCode,
            IsActive = true
        };

        db.Members.Add(member);
        await db.SaveChangesAsync(ct);

        return new MemberResponse
        {
            Id = member.Id,
            UserName = user.UserName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            FirstName = member.FirstName,
            LastName = member.LastName,
            Address = member.Address,
            ZipCode = member.ZipCode,
            IsActive = member.IsActive
        };
    }
}