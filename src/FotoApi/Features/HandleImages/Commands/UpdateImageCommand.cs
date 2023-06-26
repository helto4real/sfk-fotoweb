using FluentValidation;
using FotoApi.Infrastructure.Security.Authorization;

namespace FotoApi.Features.HandleImages.Commands;

public record UpdateImageCommand(Guid Id, string Title, string FileName, CurrentUser Owner) : ICommand;

public class UpdateImageCommandValidator : AbstractValidator<UpdateImageCommand>
{
    public UpdateImageCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title must be provided");
        RuleFor(x => x.FileName)
            .NotEmpty()
            .WithMessage("FileName must be provided");
    }
}