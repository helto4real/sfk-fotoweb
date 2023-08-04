using FluentValidation;
using Shared.Validation;

namespace FotoApi.Infrastructure.Security.Authorization.Dto;

public record LoginAndCreateUserRequest
{
    public string UserName { get; init; } = default!;
    public string Password { get; init; } = default!;
    public string Email { get; init; } = default!;
    public string UrlToken { get; set; } = default!;
}

public class LoginAndCreateUserRequestValidator : AbstractValidator<LoginAndCreateUserRequest>
{
    public LoginAndCreateUserRequestValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty()
            .WithMessage("Användarnamn måste anges.");
        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Lösenord måste anges.");
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("En giltig e-postadress måste anges.");
        RuleFor(x => x.Password)
            .AppPassword();
    }
}