// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace FotoApi.Features.HandleSubmissions.HandleStBilder.Dto;

public class StBildResponse
{
    public Guid Id { get; init; }
    public string Title { get; init; } = default!;
    public string Location { get; init; } = default!;
    public DateTime Time { get; init; }
    public string Description { get; init; } = default!;
    public Guid ImageReference { get; init; }
    public string Name { get; init; } = default!;
    public string AboutThePhotographer { get; init; } = default!;
    public bool IsUsed { get; init; }
    public bool IsAccepted { get; init; }
}