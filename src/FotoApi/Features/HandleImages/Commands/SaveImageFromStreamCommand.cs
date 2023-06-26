using FluentValidation;
using FotoApi.Features.HandleImages.Dto;
using FotoApi.Infrastructure.Security.Authorization;

namespace FotoApi.Features.HandleImages.Commands;

public record SaveImageFromStreamCommand
    (Stream Stream, string Title, string? MetadataType, string? Metadata, string FileName, CurrentUser Owner) : ICommand<ImageResponse>;

public class SaveImageFromStreamValidator : AbstractValidator<SaveImageFromStreamCommand>
{
    public SaveImageFromStreamValidator()
    {
        RuleFor(x => x.FileName)
            .NotEmpty()
            .WithMessage("FileName must be provided");
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title must be provided");
        RuleFor(x => x.Metadata)
            .NotEmpty()
            .When(x => !string.IsNullOrEmpty(x.MetadataType))
            .WithMessage("Both metadata type and metadata must be provided if any of them is provided");
        //     .
        //     .WithMessage("Metadata type must be provided");
        // RuleFor(x => x.Metadata)
        //     .NotEmpty()
        //     .WithMessage("Metadata must be provided");

    }
}