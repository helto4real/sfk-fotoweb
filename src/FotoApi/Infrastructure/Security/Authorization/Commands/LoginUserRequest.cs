using FluentValidation;
using FotoApi.Abstractions.Messaging;
using FotoApi.Infrastructure.Security.Authorization.Dto;

namespace FotoApi.Infrastructure.Security.Authorization.Commands;

public record LoginUserRequest(string UserName, string Password) : ICommand<UserAuthorizedResponse>;

public class LoginUserCommandValidator : AbstractValidator<LoginUserRequest>
{
    public LoginUserCommandValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty()
            .WithMessage("Användarnamn måste anges.");
        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Lösenord måste anges.")
            .MaximumLength(32)
            .MinimumLength(6)
            .WithMessage("Lösenordet måste vara mellan 6 och 32 tecken.")
            .Matches("^(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z])(?=.*[^a-zA-Z\\d]).*$")
            .WithMessage("Lösenordet måste innehålla små och stora bokstäver, siffror samt specialtecken.");

    }
}