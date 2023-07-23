using FluentValidation;
using FotoApi.Infrastructure.Security.Authorization;

namespace FotoApi.Features.HandleImages.Commands;

public record UpdateImageRequest(Guid Id, string Title, string FileName);

public class UpdateImageRequestValidator : AbstractValidator<UpdateImageRequest>
{
    public UpdateImageRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title must be provided");
        RuleFor(x => x.FileName)
            .NotEmpty()
            .WithMessage("FileName must be provided");
    }
}