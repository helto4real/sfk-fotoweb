namespace Foto.WebServer.Dto;


public record StBildPackageInfo
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int PackageNumber { get; init; }
    public bool IsDelivered { get; init; }
    public DateTime UpdatedDate { get; set; }
}
