namespace FotoApi.Features.HandleSubmissions.HandleStBilder.Dto;

public record StBildPackageResponse
{
    public Guid Id { get; init; } 
    public int PackageNumber { get; init; }
    public bool IsDelivered { get; init; }
    public DateTime UpdatedDate { get; init; }
}