using FluentValidation;
using FotoApi.Model;

namespace FotoApi.Features.HandleUrlTokens.Dto;

public sealed record UrlTokenTypeRequest(UrlTokenType UrlTokenType);

public class UrlTokenTypeRequestValidator : AbstractValidator<UrlTokenTypeRequest>
{
    public UrlTokenTypeRequestValidator()
    {
        RuleFor(x => x.UrlTokenType)
            .IsInEnum()
            .WithMessage("UrlTokenType must be a valid UrlTokenType");
    }
}