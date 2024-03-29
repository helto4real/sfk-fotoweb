﻿using FluentValidation;

namespace FotoApi.Features.HandleSubmissions.HandleStBilder.Dto;

public record NewStBildRequest
{
    public Guid ImageReference { get; set; }
    public string OwnerReference { get; set; } = default!;
    public string Title { get; init; } = default!;
    public string Name { get; init; } = default!;
    public string Location { get; init; } = default!;
    public DateTime Time { get; init; }
    public string Description { get; init; } = default!;
    public string AboutThePhotographer { get; init; } = default!;
}

public class NewStBildRequestValidator : AbstractValidator<NewStBildRequest>
{
    public NewStBildRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().WithMessage("Title must be provided");
        RuleFor(x => x.Location).NotEmpty().MaximumLength(50)
            .WithMessage("Location cannot be empty and must be less than 50 characters");
        RuleFor(x => x.Description).NotEmpty().MaximumLength(300)
            .WithMessage("Description cannot be empty and must be less than 300 characters");
        RuleFor(x => x.Name).NotEmpty().MaximumLength(50)
            .WithMessage("Name cannot be empty and must be less than 50 characters");
        RuleFor(x => x.AboutThePhotographer).MaximumLength(300)
            .WithMessage("CameraBrand must be less than 50 characters");
        RuleFor(x => x.Time).NotEmpty().GreaterThanOrEqualTo(new DateTime(1900, 1, 1))
            .WithMessage("Time must be after 1900-01-01");
        RuleFor(x => x.Time).NotEmpty().LessThanOrEqualTo(DateTime.Now.AddDays(1))
            .WithMessage("Time cannot be in the future");
        RuleFor(x => x.Time.Kind).Equal(DateTimeKind.Utc).WithMessage("Time must be in UTC");
    }
}