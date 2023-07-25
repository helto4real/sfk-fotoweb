using FotoApi.Features.HandleImages.Exceptions;
using FotoApi.Infrastructure.Repositories;
using FotoApi.Infrastructure.Repositories.PhotoServiceDbContext;
using FotoApi.Infrastructure.Security.Authorization;

namespace FotoApi.Features.HandleImages.Commands;

public class UpdateImageHandler(PhotoServiceDbContext db, IPhotoStore photoStore, CurrentUser currentUser)
    : IHandler<UpdateImageRequest>
{
    public async Task Handle(UpdateImageRequest request, CancellationToken cancellationToken)
    {
        var rowsAffected = await db.Images
            .Where(t => t.Id == request.Id && (t.OwnerReference == currentUser.Id || currentUser.IsAdmin))
            .ExecuteUpdateAsync(updates =>
                updates.SetProperty(t => t.Title, request.Title));

        if (rowsAffected == 0)
            throw new ImageNotFoundException(request.Id);
    }
}