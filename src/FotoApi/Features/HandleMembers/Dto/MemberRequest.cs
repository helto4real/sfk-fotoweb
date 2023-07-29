using FotoApi.Api;

namespace FotoApi.Features.HandleMembers.Dto;

public record MemberRequest
{
    public Guid Id { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string? Address { get; set; }
    public string? ZipCode { get; set; }
    public string? City { get; set; }
    public List<RoleRequest> Roles { get; set; } = new();
}