namespace FotoApi.Features.HandleUsers.Dto;

public record UserResponse
{
    public string UserName { get; init; } = default!;
    public string FirstName { get; init; } = default!;
    public string LastName { get; init; } = default!;
    public string Email { get; init; } = default!;
    public IReadOnlyCollection<string> Roles { get; set; } = default!;
}