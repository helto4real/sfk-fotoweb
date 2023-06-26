using System.ComponentModel.DataAnnotations;

namespace FotoApi.Model;

public class Member
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string OwnerReference { get; set; } = default!;
    public string FirstName { get; set; } = default!;
    public string SureName { get; set; } = default!;
}