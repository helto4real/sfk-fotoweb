using FotoApi.Features.HandleMembers.Dto;
using FotoApi.Features.HandleMembers.Exceptions;
using FotoApi.Features.HandleUsers.Exceptions;
using FotoApi.Infrastructure.Repositories.PhotoServiceDbContext;
using FotoApi.Infrastructure.Security.Authorization;
using FotoApi.Infrastructure.Security.Authorization.Dto;
using FotoApi.Infrastructure.Security.Authorization.Exceptions;
using FotoApi.Model;
using Microsoft.AspNetCore.Identity;

namespace FotoApi.Features.HandleMembers.CommandHandlers;

public class UpdateMembersHandler
    (PhotoServiceDbContext db, UserManager<User> userManager, CurrentUser currentUser, ILogger<UpdateMembersHandler> logger) : IHandler<MemberRequest, MemberResponse>
{
    public async Task<MemberResponse> Handle(MemberRequest request, CancellationToken ct = default)
    {
        var member = await db.Members.FindAsync(new object?[] { request.Id, ct }, cancellationToken: ct);
        if (member is null) throw new MemberNotFoundException(request.Id);
        
        var user = await userManager.FindByIdAsync(member.OwnerReference);

        if (user is null)
        {
            logger.LogError("Failed get user for member {Member} with owner reference {Reference}", member.Id, member.OwnerReference);
            throw new MemberException("Systemfel: Användare finns inte för medlem, kontakta administratören.");
        }

        if (!currentUser.IsAdmin && currentUser.User!.UserName != user.UserName)
        {
            throw new UserNotAuthorizedException("Du har inte behörighet att uppdatera denna medlem.");
        }

        var userUpdated = false;
        if (user.PhoneNumber != request.PhoneNumber)
        {
            user.PhoneNumber = request.PhoneNumber;
            userUpdated = true;
        }

        if (request.Email is not null &&  user.Email != request.Email)
        {
            var checkUserWithEmailExist = await userManager.FindByEmailAsync(request.Email);
            if (checkUserWithEmailExist is not null)
            {
                throw new MemberException($"En användare med E-post {request.Email} finns redan.");
            }
            user.Email = request.Email;
            user.EmailConfirmed = false;
            userUpdated = true;
        }

        if (userUpdated)
        {
            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                logger.LogError("Failed to update user Error: {Error}", result.Errors);
                throw new MemberException("Systemfel: Kunde inte uppdatera användarinformation, kotakta administratören.");
            }

        }
        await AddRolesToUser(userManager, user, request.Roles);
        
        member.FirstName = request.FirstName;
        member.LastName = request.LastName;
        member.Address = request.Address;
        member.ZipCode = request.ZipCode;
        member.City = request.City;
        member.IsActive = true;
        
        db.Members.Update(member);
        await db.SaveChangesAsync(ct);

        return new MemberResponse
        {
            Id = member.Id,
            UserName = user.UserName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Address = request.Address,
            ZipCode = request.ZipCode,
            City = request.City,
            IsActive = member.IsActive,
            Roles = request.Roles.Select(n => new RoleResponse {Name = n.Name}).ToList()
        };
    }

    static async Task AddRolesToUser(UserManager<User> userManager, User user, IReadOnlyCollection<RoleRequest> roles)
    {
        var currentRoles = await userManager.GetRolesAsync(user);
        var rolesToAdd = roles.Where(r => !currentRoles.Contains(r.Name)).ToList();
        var rolesToDelete = currentRoles.Where(r => !roles.Select(n => n.Name).Contains(r)).ToList();
        
        if (rolesToAdd.Count != 0)
        {
            var result = await userManager.AddToRolesAsync(user, rolesToAdd.Select(r => r.Name));
            if (!result.Succeeded) throw new UserException(result.Errors.Select(e => e.Description));
        }
        if (rolesToDelete.Count != 0)
        {
            var result = await userManager.RemoveFromRolesAsync(user, rolesToDelete);
            if (!result.Succeeded) throw new UserException(result.Errors.Select(e => e.Description));
        }
    }
}