using System.ComponentModel.DataAnnotations;

namespace FotoApi.Model;

public record Member : TimeTrackedEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string OwnerReference { get; set; } = default!;
    public string FirstName { get; set; } = default!;
    public string SureName { get; set; } = default!;
}