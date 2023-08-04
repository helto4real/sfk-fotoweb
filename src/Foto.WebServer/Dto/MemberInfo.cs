using FluentValidation;
using Shared.Validation;

namespace Foto.WebServer.Dto;

public record MemberInfo
{
    public Guid Id { get; set; }
    public string? UserName { get; set; }
    public string Email { get; set; } = default!;
    public string? PhoneNumber { get; set; }
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string? Address { get; set; }
    public string? ZipCode { get; set; }
    public string? City { get; set; }
    public bool IsActive { get; set; }
    public List<RoleInfo> Roles { get; set; } = new();
}

public class MemberInfoValidator : AbstractValidator<MemberInfo>
{
    public MemberInfoValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("Förnamn måste anges.");
        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Efternamn måste anges.");
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("En giltig e-postadress måste anges.");
        RuleFor(x => x.PhoneNumber)
            .AppPhoneNumber();
    }
}