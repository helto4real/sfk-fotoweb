namespace FotoApi.Features.HandleStBilder.Commands;

public record AcceptStBildCommand(Guid StBildId, bool StBildAcceptStatus) : ICommand;