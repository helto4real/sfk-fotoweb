using FotoApi.Infrastructure.Validation.Exceptions;

namespace FotoApi.Features.HandleImages.Exceptions;

public sealed class BadImageMetadataTypeException : NotFoundException
{
    public BadImageMetadataTypeException(string? imageMetadataType)
        : base($"Bad image metadata type {imageMetadataType}.")
    {
    }
}