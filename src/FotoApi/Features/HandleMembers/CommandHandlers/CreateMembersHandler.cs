using FotoApi.Features.HandleMembers.Dto;
using FotoApi.Features.HandleUsers.Exceptions;
using FotoApi.Features.Shared.Exceptions;
using FotoApi.Infrastructure.Repositories.PhotoServiceDbContext;
using FotoApi.Infrastructure.Security.Authorization.Dto;
using FotoApi.Model;
using Microsoft.AspNetCore.Identity;

namespace FotoApi.Features.HandleMembers.CommandHandlers;

public class CreateMembersHandler
    (PhotoServiceDbContext db, UserManager<User> userManager, ILogger<CreateMembersHandler> logger) : IHandler<NewMemberRequest, MemberResponse>
{
    public async Task<MemberResponse> Handle(NewMemberRequest request, CancellationToken ct = default)
    {
        
        var (user, userWasCreated) = await GetOrCreateUser(request);
        if (!userWasCreated)
        {
            // User already exists, now we check if a member exists with that user as owner
            if (db.Members.Any(m => m.OwnerReference == user.Id))
                throw new UserException("Member with that email already exists");
        }
        
        await UpdateUserIfInfoUpdated(request, user);

        await AddRolesToUser(userManager, user, request.Roles);

        var member = new Member
        {
            OwnerReference = user.Id,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Address = request.Address,
            ZipCode = request.ZipCode,
            City = request.City,
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
            City = member.City,
            IsActive = member.IsActive,
            Roles = request.Roles.Select(n => new RoleResponse {Name = n.Name}).ToList()
        };
    }
    
    async Task AddRolesToUser(UserManager<User> userManager, User user, IReadOnlyCollection<RoleRequest> roles)
    {
        var currentRoles = await userManager.GetRolesAsync(user);
        var rolesToAdd = roles.Where(r => !currentRoles.Contains(r.Name)).ToList();
        var rolesToDelete = currentRoles.Where(r => !roles.Select(n => n.Name).Contains(r)).ToList();
        
        if (rolesToAdd.Any())
        {
            var result = await userManager.AddToRolesAsync(user, rolesToAdd.Select(r => r.Name));
            if (!result.Succeeded) throw new UserException(result.Errors.Select(e => e.Description));
        }
        if (rolesToDelete.Any())
        {
            var result = await userManager.RemoveFromRolesAsync(user, rolesToDelete);
            if (!result.Succeeded) throw new UserException(result.Errors.Select(e => e.Description));
        }
    }

    private async ValueTask UpdateUserIfInfoUpdated(NewMemberRequest newMemberRequest, User user)
    {
        var userUpdated = false;
        if (user.PhoneNumber != newMemberRequest.PhoneNumber)
        {
            user.PhoneNumber = newMemberRequest.PhoneNumber;
            userUpdated = true;
        }

        if (user.Email != newMemberRequest.Email)
        {
            user.Email = newMemberRequest.Email;
            userUpdated = true;
        }
        
        if (userUpdated)
        {
            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                logger.LogError("Failed to update user Error: {Error}", result.Errors);
                throw new SystemErrorException("Systemfel: Kunde inte uppdatera användarinformation, kontakta administratören.");
            }
        }
    }
    private async Task<(User, bool)> GetOrCreateUser(NewMemberRequest request)
    {
        // Both username and email are unique in the system this is why we check both
        // First we check if user email has a user with that username
        
        var userWasCreated = false;
        // Then we check if there are an existing ser with that email
        var user = await userManager.FindByNameAsync(request.Email) ?? await userManager.FindByEmailAsync(request.Email);

        if (user is not null) return (user, userWasCreated);
        // There are no user with that email as user name or email, so we create a new user
        user = new User
        {
            UserName = request.Email,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            RefreshToken = "",
            RefreshTokenExpirationDate = DateTime.MinValue
        };
        var result = await userManager.CreateAsync(user);
        if (!result.Succeeded)
        {
            logger.LogError("Failed to create user Error: {Error}", result.Errors);
            throw new SystemErrorException("Systemfel: Kunde inte skapa användare, kotakta administratören.");
        }

        userWasCreated = true;

        return (user, userWasCreated);
    }
}