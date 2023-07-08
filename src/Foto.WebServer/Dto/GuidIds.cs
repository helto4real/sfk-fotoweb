namespace Foto.WebServer.Dto;

public record GuidIds
{
    public List<Guid> Ids { get; init; } = default!;
}