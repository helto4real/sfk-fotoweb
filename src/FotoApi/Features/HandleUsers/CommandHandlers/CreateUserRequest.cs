using FluentValidation;
using FotoApi.Features.HandleUsers.Dto;

namespace FotoApi.Features.HandleUsers.CommandHandlers;

public record CreateUserRequest
{
    public string UserName { get; init; } = default!;

    public string Password { get; init; } = default!;
    public string FirstName { get; init; } = default!;
    public string LastName { get; init; } = default!;
    public string Email { get; init; } = default!;

    public string UrlToken { get; set; } = default!;
}

