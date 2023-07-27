using FotoApi.Infrastructure.Validation.Exceptions;

namespace FotoApi.Features.HandleSubmissions.HandleStBilder.Exceptions;

public sealed class StBildNotFoundException : NotFoundException
{
    public StBildNotFoundException(Guid imageId)
        : base($"The st-bild with the identifier {imageId} was not found.")
    {
    }
}