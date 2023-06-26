using FluentValidation;
using FotoApi.Features.HandleUrlTokens.Model;
using FotoApi.Model;

namespace FotoApi.Features.HandleUrlTokens.Commands;

public sealed record AddUrlTokenFromUrlTokenTypeCommand(UrlTokenType UrlTokenType) : ICommand<UrlTokenResponse>;

public class AddUrlTokenFromUrlTokenTypeCommandValidator : AbstractValidator<AddUrlTokenFromUrlTokenTypeCommand>
{
    public AddUrlTokenFromUrlTokenTypeCommandValidator()
    {
        RuleFor(x => x.UrlTokenType)
            .IsInEnum()
            .WithMessage("UrlTokenType must be a valid UrlTokenType");
    }
}