namespace Foto.WebServer.Dto;

public record ImageItem
{
    public Guid Id { get; init; }
    public string Title { get; init; } = default!;
}