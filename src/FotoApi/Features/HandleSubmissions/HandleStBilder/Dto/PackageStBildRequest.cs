using System.Collections.ObjectModel;

namespace FotoApi.Features.HandleSubmissions.HandleStBilder.Dto;

public class PackageStBildRequest
{
    public IReadOnlyCollection<Guid> Ids { get; init; } = ReadOnlyCollection<Guid>.Empty;
}