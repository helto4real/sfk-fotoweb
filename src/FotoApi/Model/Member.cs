using System.ComponentModel.DataAnnotations;

namespace FotoApi.Model;

public record Member : TimeTrackedEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string OwnerReference { get; set; } = default!;
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string AboutMe { get; set; } = default!;
    DateTime FeePayDate { get; set; } = default;
    public string Phonenumber { get; set; } = default!;
}