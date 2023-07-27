namespace FotoApi.Infrastructure.Security.Authorization.Dto;

public record LoginExternalUserCommand(
    string UserName,
    string Provider,
    string ProviderKey,
    string UrlToken);

