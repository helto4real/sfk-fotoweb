namespace FotoApi.Features.HandleSubmissions.HandleStBilder.Dto;

public class StBildResponse
{
    public Guid Id { get; init; } = default!;
    public string Title { get; init; } = default!;
    public string Location { get; init; } = default!;
    public DateTime Time { get; init; } = default;
    public string Description { get; init; } = default!;
    public Guid ImageReference { get; init; } = default!;
    public string Name { get; init; } = default!;
    public string AboutThePhotograper { get; init; } = default!;
    public bool IsUsed { get; init; } 
    public bool IsAccepted { get; init; }
}