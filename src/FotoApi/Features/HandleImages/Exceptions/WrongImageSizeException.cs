using FotoApi.Infrastructure.Validation.Exceptions;

namespace FotoApi.Features.HandleImages.Exceptions;

public sealed class WrongImageSizeException : BadRequestException
{
    public WrongImageSizeException(int width, int height)
        : base($"The image has wrong size, expeced {width}x{height}.")
    {
    }
}