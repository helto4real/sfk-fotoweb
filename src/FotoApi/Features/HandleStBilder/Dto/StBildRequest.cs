namespace FotoApi.Features.HandleStBilder.Dto;

public record StBildRequest
{
    public string Title { get; init; } = default!;
    public string Name { get; init; } = default!;
    public string Location { get; init; } = default!;
    public DateTime Time { get; init; } = default;
    public string Description { get; init; } = default!;
    public string AboutThePhotograper { get; init; } = default!;
    public bool IsAccepted { get; init; }
}