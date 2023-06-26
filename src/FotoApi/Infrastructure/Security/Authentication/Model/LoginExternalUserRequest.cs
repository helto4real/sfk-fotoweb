namespace FotoApi.Infrastructure.Security.Authentication.Model;

public record LoginExternalUserRequest (string UserName, string ProviderKey, string UrlToken);
