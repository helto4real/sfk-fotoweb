using System.Collections.ObjectModel;
using FotoApi.Infrastructure.Security.Authorization.Dto;

namespace FotoApi.Features.HandleMembers.Dto;

public record MemberResponse
{
    public Guid Id { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string? Address { get; set; }
    public string? ZipCode { get; set; }
    public string? City { get; set; }
    public IReadOnlyCollection<RoleResponse> Roles { get; set; } = ReadOnlyCollection<RoleResponse>.Empty; 
    public bool IsActive { get; set; }
}

