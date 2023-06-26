using FluentValidation;

namespace FotoApi.Features.HandleUsers.Commands;

public record ConfirmEmailCommand(string UrlToken) : ICommand;

public class ConfirmEmailValidator : AbstractValidator<ConfirmEmailCommand>
{
    public ConfirmEmailValidator()
    {
        RuleFor(x => x.UrlToken)
            .NotEmpty()
            .WithMessage("Urltoken must be provided");
    }
}