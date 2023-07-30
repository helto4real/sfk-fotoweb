namespace FotoApi.Features.HandleMembers.Dto;

public record MemberListItemResponse
{
    public Guid Id { get; init; }
    public string UserName { get; init; } = default!;
    public string FirstName { get; init; } = default!;
    public string LastName { get; init; } = default!;
    public string Email { get; init; } = default!;
    public bool IsActive { get; set; }
}