using FotoApi.Model;

namespace FotoApi.Features.HandleUrlTokens.Model;

public record UrlTokenResponse
{
    public Guid Id { get; init; }
    public string Token { get; set; } = default!;
    public UrlTokenType UrlTokenType { get; set; } = default!;

    public DateTime ExpirationDate { get; set; } = default!;
}