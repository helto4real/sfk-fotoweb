using FotoApi.Infrastructure.Validation.Exceptions;

namespace FotoApi.Features.HandleSubmissions.HandleStBilder.Exceptions;

public class PackageNotFoundException : NotFoundException
{
    public PackageNotFoundException(Guid id) : base($"Package with id {id} not found!")
    {
    }
}