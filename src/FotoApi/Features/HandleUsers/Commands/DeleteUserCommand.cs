using FluentValidation;

namespace FotoApi.Features.HandleUsers.Commands;

public record DeleteUserCommand(string UserName) : ICommand;

public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserCommandValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty()
            .WithMessage("Username must be provided");
    }
}