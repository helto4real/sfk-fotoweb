using System.Text.Json.Serialization;
using FluentValidation;
using Foto.WebServer.Shared;
using Shared.Validation;

namespace Foto.WebServer.Dto;

/// <summary>
///     Used to update login information
/// </summary>
public record UpdateLoginInfo
{
    public string? NewUserName { get; set; }
    public string? CurrentPassword { get; set; }
    public string? NewPassword { get; set; }
    public string? NewEmail { get; set; }
    public string? ConfirmNewEmail { get; set; }
    public bool IsUserExternal { get; set; }

    [JsonIgnore] public string? ConfirmPassword { get; set; }
    [JsonIgnore] public string? CurrentUserName { get; init; }
    [JsonIgnore] public string? CurrentEmail { get; init; }
}

public class UpdateLoginInfoValidator : ValidatorBase<UpdateLoginInfo>
{
    public UpdateLoginInfoValidator(bool isExternal)
    {
        
        When(x => !string.IsNullOrWhiteSpace(x.NewPassword), () =>
        {
            RuleFor(x => x.CurrentPassword).NotEmpty().WithMessage("Password is required");
            RuleFor(x => x.NewPassword)
                .AppPassword();
            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.NewPassword)
                .WithMessage("Lösenorden måste matcha.");
        });

        When(x => !string.IsNullOrWhiteSpace(x.NewUserName), () =>
        {
            RuleFor(x => x.NewUserName).EmailAddress()
                .WithMessage("Användarnamnet måste vara en giltig e-postadress");
            RuleFor(x => x.NewUserName).MaximumLength(50)
                .WithMessage("Username must be at most 50 characters");
        });

        When(x => !string.IsNullOrWhiteSpace(x.NewEmail),
            () =>
            {
                if (!isExternal)
                {
                    RuleFor(x => x.CurrentPassword)
                        .NotEmpty()
                        .WithMessage("Du måste ange lösenordet för att ändra dina uppgifter.");
                }

                RuleFor(x => x.NewEmail).EmailAddress().WithMessage("E-post måste vara en giltig e-postadress");
                RuleFor(x => x.ConfirmNewEmail).Equal(x => x.NewEmail)
                    .WithMessage("E-postadresserna måste matcha.");
            });
    }
}