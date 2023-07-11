using FluentValidation;

namespace FotoApi.Infrastructure.Security.Authentication.Model;

public record LoginUserRequest (string UserName, string Password);

    public class LoginUserRequestValidator : AbstractValidator<LoginUserRequest>
    {
        public LoginUserRequestValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty()
                .WithMessage("UserName must be provided");
            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password must be provided")
                .MaximumLength(32)
                .MinimumLength(6)
                .WithMessage("Lösenordet måste vara mellan 6 och 32 tecken.")
                .Matches("^(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z])(?=.*[^a-zA-Z\\d]).*$")
                .WithMessage("Lösenordet måste innehålla små och stora bokstäver, siffror samt specialtecken.");

        }
    }