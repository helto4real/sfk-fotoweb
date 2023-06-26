using FotoApi.Infrastructure.Validation.Exceptions;

namespace FotoApi.Features.HandleUrlTokens.Exceptions;

public sealed class UrlTokenNotFoundException : NotFoundException
{
    public UrlTokenNotFoundException(Guid urlTokenId)
        : base($"The UrlToken with the identifier {urlTokenId} was not found.")
    {
    }

    public UrlTokenNotFoundException(string tokenString)
        : base($"The UrlToken with the token string {tokenString} was not found.")
    {
    }
}