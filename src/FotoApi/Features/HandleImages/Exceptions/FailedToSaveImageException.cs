using FotoApi.Infrastructure.Validation.Exceptions;

namespace FotoApi.Features.HandleImages.Exceptions;

public sealed class FailedToSaveImageException : BadRequestException
{
    public FailedToSaveImageException(string filename)
        : base($"The image with the filename {filename} could not be saved.")
    {
    }
}