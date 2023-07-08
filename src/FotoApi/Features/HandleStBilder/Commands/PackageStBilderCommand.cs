using FotoApi.Infrastructure.Security.Authorization;

namespace FotoApi.Features.HandleStBilder.Commands;

public record PackageStBilderCommand(List<Guid> StBildIds, CurrentUser Owner) : ICommand<bool>;