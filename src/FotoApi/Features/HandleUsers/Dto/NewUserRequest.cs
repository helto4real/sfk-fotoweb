using FluentValidation;

namespace FotoApi.Features.HandleUsers.Dto;

public class NewUserRequest
{
    public string UserName { get; init; } = default!;
    public string FirstName { get; init; } = default!;
    public string Password { get; init; } = default!;
    public string LastName { get; init; } = default!;
    public string Email { get; init; } = default!;
    public string UrlToken { get; set; } = default!;
}

public class NewUserValidator : AbstractValidator<NewUserRequest>
{
    public NewUserValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty()
            .WithMessage("Username must be provided")
            .MaximumLength(50)
            .WithMessage("Username must be less than 50 characters");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password must be provided")
            .MaximumLength(32)
            .MinimumLength(6)
            .WithMessage("Password must be between 6 and 32 characters")
            .Matches("^(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z])(?=.*[^a-zA-Z\\d]).*$")
            .WithMessage(
                "Password must contain at least one uppercase letter, one lowercase letter, one number and one special character");

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("First name must be provided")
            .MaximumLength(50)
            .WithMessage("First name must be less than 50 characters");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Last name must be provided")
            .MaximumLength(50)
            .WithMessage("Last name must be less than 50 characters");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email must be provided")
            .EmailAddress()
            .WithMessage("Email must be a valid email address");
    }
}