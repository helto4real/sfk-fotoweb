﻿using FluentValidation;
using FotoApi.Infrastructure.Security.Authorization.Dto;

namespace FotoApi.Features.HandleMembers.Dto;

public record MemberRequest
{
    public Guid Id { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string? Address { get; set; }
    public string? ZipCode { get; set; }
    public string? City { get; set; }
    public List<RoleRequest> Roles { get; set; } = new();
}

public class MemberRequestValidator : AbstractValidator<MemberRequest>
{
    public MemberRequestValidator()
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
            .NotEmpty()
            .Matches(@"^(07|01)[\d]{1}-?[\d]{7}$")
            .WithMessage("Giltiga telefonnummer är bara siffror eller med ett streck efter riktnummer.");
    }
}