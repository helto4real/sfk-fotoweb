using FluentValidation;
using FotoApi.Features.HandleUsers.CommandHandlers;

namespace FotoApi.Features.HandleUsers.Dto;

public class UserRequest
{
    public string UserName { get; init; } = default!;
    public string FirstName { get; init; } = default!;
    public string LastName { get; init; } = default!;
    public string Email { get; init; } = default!;
    public bool IsAdmin { get; set; }
}

public class UserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UserRequestValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty()
            .WithMessage("Username must be provided")
            .MaximumLength(50)
            .WithMessage("Username must be less than 50 characters");
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