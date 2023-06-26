using System.ComponentModel.DataAnnotations;

namespace FotoApi.Model;

public record UrlToken
{
    public Guid Id { get; init; }
    public string Token { get; set; } = default!;
    public DateTime ExpirationDate { get; set; }
    public UrlTokenType UrlTokenType { get; set; }
    public string Data { get; set; } = default!;
}