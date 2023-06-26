using FotoApi.Abstractions.Messaging;
using FotoApi.Infrastructure.Security.Authorization.Dto;

namespace FotoApi.Infrastructure.Security.Authorization.Commands;

public record LoginUserCommand(string UserName, string Password, bool IsAdmin) : ICommand<AuthorizationTokenResponse>;