﻿using FotoApi.Features.HandleImages.Exceptions;
using FotoApi.Infrastructure.Repositories.PhotoServiceDbContext;
using FotoApi.Infrastructure.Security.Authorization;

namespace FotoApi.Features.HandleImages.CommandHandlers;

public class UpdateImageHandler(PhotoServiceDbContext db, CurrentUser currentUser)
    : IHandler<UpdateImageRequest>
{
    public async Task Handle(UpdateImageRequest request, CancellationToken ct)
    {
        var rowsAffected = await db.Images
            .Where(t => t.Id == request.Id && (t.OwnerReference == currentUser.Id || currentUser.IsAdmin))
            .ExecuteUpdateAsync(updates =>
                updates.SetProperty(t => t.Title, request.Title), cancellationToken: ct);

        if (rowsAffected == 0)
            throw new ImageNotFoundException(request.Id);
    }
}