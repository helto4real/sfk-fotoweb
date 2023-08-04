namespace FotoApi.Features.HandleUsers.Dto;

public record UserResponse
{
    public string UserName { get; init; } = default!;
    public string Email { get; init; } = default!;
    public bool EmailConfirmed { get; init; }
    public IReadOnlyCollection<string> Roles { get; set; } = default!;
}