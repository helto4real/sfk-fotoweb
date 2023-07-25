using FluentValidation;

namespace FotoApi.Features.HandleStBilder.Dto;

public record StBildRequest
{
    public Guid Id { get; init; }
    public string Title { get; init; } = default!;
    public string Name { get; init; } = default!;
    public string Location { get; init; } = default!;
    public DateTime Time { get; init; } = default;
    public string Description { get; init; } = default!;
    public string AboutThePhotograper { get; init; } = default!;
    public bool IsAccepted { get; init; }
}

public class StBildRequestValidator : AbstractValidator<StBildRequest>
{
    public StBildRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().WithMessage("Title must be provided");
        RuleFor(x => x.Location).NotEmpty().MaximumLength(50)
            .WithMessage("Location cannot be empty and must be less than 50 characters");
        RuleFor(x => x.Description).NotEmpty().MaximumLength(300)
            .WithMessage("Description cannot be empty and must be less than 300 characters");
        RuleFor(x => x.Name).NotEmpty().MaximumLength(50)
            .WithMessage("Name cannot be empty and must be less than 50 characters");
        RuleFor(x => x.AboutThePhotograper).MaximumLength(300).WithMessage("CameraBrand must be less than 50 characters");
        RuleFor(x => x.Time).NotEmpty().GreaterThanOrEqualTo(new DateTime(1900, 1, 1)).WithMessage("Time must be after 1900-01-01");
        RuleFor(x => x.Time).NotEmpty().LessThanOrEqualTo(DateTime.Now.AddDays(1)).WithMessage("Time cannot be in the future");
        RuleFor(x => x.Time.Kind).Equal(DateTimeKind.Utc).WithMessage("Time must be in UTC");
    }
}

