using FotoApi.Infrastructure.Validation.Exceptions;

namespace FotoApi.Features.HandleImages.Exceptions;

public sealed class ImageNotFoundException : NotFoundException
{
    public ImageNotFoundException(Guid urlTokenId)
        : base($"The image with the identifier {urlTokenId} was not found.")
    {
    }
}