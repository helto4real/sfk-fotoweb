﻿using FluentValidation;
using Shared.Validation;

namespace Foto.WebServer.Dto;

public record NewMemberInfo(
    string Email,
    string? PhoneNumber,
    string FirstName,
    string LastName,
    string? Address,
    string? ZipCode,
    string? City,
    List<RoleInfo> Roles);
    
public class NewMemberInfoValidator : AbstractValidator<NewMemberInfo>
{
    public NewMemberInfoValidator()
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