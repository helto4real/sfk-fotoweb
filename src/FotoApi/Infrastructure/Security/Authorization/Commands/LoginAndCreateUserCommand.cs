using FotoApi.Infrastructure.Security.Authorization.Dto;

namespace FotoApi.Infrastructure.Security.Authorization.Commands;

public record LoginAndCreateUserCommand
{
    public string UserName { get; init; } = default!;

    public string Password { get; init; } = default!;
    public string FirstName { get; init; } = default!;
    public string LastName { get; init; } = default!;
    public string Email { get; init; } = default!;

    public string UrlToken { get; set; } = default!;
}