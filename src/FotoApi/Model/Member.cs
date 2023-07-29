using System.ComponentModel.DataAnnotations;

namespace FotoApi.Model;

public record Member : TimeTrackedEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string OwnerReference { get; set; } = default!;
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string? AboutMe { get; set; }
    public DateTime? FeePayDate { get; set; }
    public string? Address { get; set; }
    public string? ZipCode { get; set; }
    public string? City { get; set; }
    
    public bool IsActive { get; set; }
}