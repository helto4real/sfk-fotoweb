using FluentValidation;

namespace FotoApi.Features.HandleUsers.Dto;

public record EmailRequest(string Email);

public class EmailRequestValidator : AbstractValidator<EmailRequest>
{
    public EmailRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Användarnamn i form av e-post måste anges")
            .EmailAddress()
            .WithMessage("Användarnamn måste vara en giltig e-postadress");
    }
}