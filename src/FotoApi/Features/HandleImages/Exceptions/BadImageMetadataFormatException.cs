using FotoApi.Infrastructure.Validation.Exceptions;

namespace FotoApi.Features.HandleImages.Exceptions;

public sealed class BadImageMetadataFormatException : NotFoundException
{
    public BadImageMetadataFormatException(string? imageMetadataType, string? metadata)
        : base($"Failed to parse metadata form type {imageMetadataType}. Metadata provided: {metadata}")
    {
    }
}