using FluentValidation;
using FotoApi.Features.Shared.Dto;
using FotoApi.Infrastructure.Security.Authorization;

namespace FotoApi.Features.HandleStBilder.Commands;

public record NewStBildCommand : ICommand<IdentityResponse>
{
    public Guid ImageReference { get; set; }
    public string OwnerReference { get; set; } = default!;
    public string Title { get; init; } = default!;
    public string Name { get; init; } = default!;
    public string Location { get; init; } = default!;
    public DateTime Time { get; init; } = default;
    public string Description { get; init; } = default!;
    public string AboutThePhotograper { get; init; } = default!;
    
}

public class NewStBildValidator : AbstractValidator<NewStBildCommand>
{
    public NewStBildValidator()
    {
        RuleFor(x => x.Title).NotEmpty().WithMessage("Title must be provided");
        RuleFor(x => x.Location).NotEmpty().MaximumLength(50)
            .WithMessage("Location cannot be empty and must be less than 50 characters");
        RuleFor(x => x.Description).NotEmpty().MaximumLength(300)
            .WithMessage("Description cannot be empty and must be less than 300 characters");
        RuleFor(x => x.Name).NotEmpty().MaximumLength(50)
            .WithMessage("Name cannot be empty and must be less than 50 characters");
        RuleFor(x => x.AboutThePhotograper).MaximumLength(300).WithMessage("CameraBrand must be less than 50 characters");
        RuleFor(x => x.Time).NotEmpty().GreaterThanOrEqualTo(new DateTime(1900, 1, 1));
    }
}