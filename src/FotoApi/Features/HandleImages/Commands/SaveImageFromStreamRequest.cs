using FluentValidation;
using FotoApi.Abstractions;
using FotoApi.Features.HandleImages.Dto;
using FotoApi.Infrastructure.Security.Authorization;

namespace FotoApi.Features.HandleImages.Commands;

public record SaveImageFromStreamRequest
    (Stream Stream, string Title, string? MetadataType, string? Metadata, string FileName) : ICurrentUser
{
    public CurrentUser CurrentUser { get; set; } = default!;
}

public class SaveImageFromStreamRequestValidator : AbstractValidator<SaveImageFromStreamRequest>
{
    public SaveImageFromStreamRequestValidator()
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
    }
}