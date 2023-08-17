using FotoApi.Model;

namespace FotoApi.Features.HandleUrlTokens.Dto;

public record UrlTokenResponse
{
    public Guid Id { get; init; }
    public string Token { get; set; } = default!;
    public UrlTokenType UrlTokenType { get; set; }

    public DateTime ExpirationDate { get; set; }
}