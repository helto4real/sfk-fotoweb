using System.Collections.ObjectModel;
using FluentValidation;
using FotoApi.Infrastructure.Security.Authorization.Dto;
using Shared.Validation;

namespace FotoApi.Features.HandleMembers.Dto;

public record NewMemberRequest
{
    public string Email { get; set; } = default!;
    public string? PhoneNumber { get; set; }
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string? Address { get; set; }
    public string? ZipCode { get; set; }
    public string? City { get; set; }
    public IReadOnlyCollection<RoleRequest> Roles { get; set; } = ReadOnlyCollection<RoleRequest>.Empty;
}

public class NewMemberRequestValidator : AbstractValidator<NewMemberRequest>
{
    public NewMemberRequestValidator()
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
        RuleFor(x => x.PhoneNumber!)
            .AppPhoneNumber();
    }
}