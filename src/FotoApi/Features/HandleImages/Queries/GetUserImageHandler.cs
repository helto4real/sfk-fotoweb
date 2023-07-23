using FotoApi.Features.HandleImages.Dto;
using FotoApi.Features.HandleImages.Exceptions;
using FotoApi.Infrastructure.Repositories;
using FotoApi.Infrastructure.Security.Authorization;

namespace FotoApi.Features.HandleImages.Queries;

public class GetUserImageHandler(PhotoServiceDbContext db, CurrentUser currentUser) : IHandler<Guid, ImageResponse>
{
    private readonly ImagesMapper _mapper = new();

    public async Task<ImageResponse> Handle(Guid imageId, CancellationToken cancellationToken)
    {
        return await db.Images.FindAsync(imageId) switch
        {
            { } image when image.OwnerReference == imageId.ToString() || currentUser.IsAdmin =>
                _mapper.ToImageResponse(image),
            _ => throw new ImageNotFoundException(imageId)
        };
    }
}