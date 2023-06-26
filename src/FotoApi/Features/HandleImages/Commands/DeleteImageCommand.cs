using FotoApi.Infrastructure.Security.Authorization;

namespace FotoApi.Features.HandleImages.Commands;

public record DeleteImageCommand(Guid Id, CurrentUser Owner) : ICommand;