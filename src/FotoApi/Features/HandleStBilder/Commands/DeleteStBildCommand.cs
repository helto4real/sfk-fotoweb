using FotoApi.Infrastructure.Security.Authorization;

namespace FotoApi.Features.HandleStBilder.Commands;

public record DeleteStBildCommand(Guid Id, CurrentUser Owner) : ICommand;