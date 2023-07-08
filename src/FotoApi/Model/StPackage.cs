namespace FotoApi.Model;

public record StPackage()
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int PackageNumber { get; init; }
    public bool IsDelivered { get; init; }
    public string PackageRelativPath { get; set; } = default!;
}

public record StPackageItem()
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid StPackageReference { get; init; }
    public Guid StBildReference { get; init; }
}