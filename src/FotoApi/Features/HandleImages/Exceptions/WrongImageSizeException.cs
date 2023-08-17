using FotoApi.Infrastructure.Validation.Exceptions;

namespace FotoApi.Features.HandleImages.Exceptions;

public sealed class WrongImageSizeException(int width, int height) : 
    BadRequestException($"The image has wrong size, expected {width}x{height}.");