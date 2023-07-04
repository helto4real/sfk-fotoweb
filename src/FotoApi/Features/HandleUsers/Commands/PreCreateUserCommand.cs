using FluentValidation;
using FotoApi.Features.HandleUsers.Dto;
using FotoApi.Model;

namespace FotoApi.Features.HandleUsers.Commands;

public record PreCreateUserCommand(string Email) : ICommand<UserResponse>;

public class PreCreateUserCommandValidator : AbstractValidator<PreCreateUserCommand>
{
    public PreCreateUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Avändarnamn i form av e-post måste anges")
            .EmailAddress()
            .WithMessage("Användarnamn måste vara en giltig e-postadress");
    }
}