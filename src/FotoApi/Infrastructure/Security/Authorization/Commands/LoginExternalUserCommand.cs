using FotoApi.Abstractions.Messaging;
using FotoApi.Infrastructure.Security.Authorization.Dto;

namespace FotoApi.Infrastructure.Security.Authorization.Commands;

public record LoginExternalUserCommand(
    string UserName, 
    string Provider, 
    string ProviderKey, 
    string UrlToken) : ICommand<AuthorizationTokenResponse>;

