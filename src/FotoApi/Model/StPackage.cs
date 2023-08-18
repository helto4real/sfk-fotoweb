namespace FotoApi.Model;

public record StPackage : TimeTrackedEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int PackageNumber { get; set; }
    public bool IsDelivered { get; set; }
    public string PackageRelativPath { get; set; } = default!;
}

public record StPackageItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid StPackageReference { get; init; }
    public Guid StBildReference { get; init; }
}