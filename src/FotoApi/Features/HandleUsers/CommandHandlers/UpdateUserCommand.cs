namespace FotoApi.Features.HandleUsers.CommandHandlers;

public class UpdateUserCommand
{
    public string UserName { get; init; } = default!;
    public string FirstName { get; init; } = default!;
    public string LastName { get; init; } = default!;
    public string Email { get; init; } = default!;
    public bool IsAdmin { get; set; }
}

