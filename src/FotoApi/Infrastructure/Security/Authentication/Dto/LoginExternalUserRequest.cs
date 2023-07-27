namespace FotoApi.Infrastructure.Security.Authentication.Dto;

public record LoginExternalUserRequest (string UserName, string ProviderKey, string UrlToken);
