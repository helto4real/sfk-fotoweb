﻿namespace FotoApi.Features.HandleUsers.CommandHandlers;

public record CreateUserRequest
{
    public string UserName { get; init; } = default!;

    public string Password { get; init; } = default!;
    public string Email { get; init; } = default!;

    public string UrlToken { get; set; } = default!;
}

