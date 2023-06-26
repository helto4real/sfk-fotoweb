namespace FotoApi.Features.HandleUrlTokens.Commands;

public sealed record DeleteTokenFromIdCommand(Guid Id) : ICommand;