namespace FotoApi.Infrastructure.Settings;

public record ApiSettings
{
    public string PhotoWebUri { get; init; } = default!;
}