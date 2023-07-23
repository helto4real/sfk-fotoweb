﻿using FotoApi.Features.HandleImages.Dto;
using FotoApi.Features.HandleImages.Exceptions;
using FotoApi.Infrastructure.Repositories;
using FotoApi.Infrastructure.Security.Authorization;
using MediatR;

namespace FotoApi.Features.HandleImages.Commands;

public class DeleteImageHandler(PhotoServiceDbContext db, IPhotoStore photoStore, CurrentUser currentUser,
        IMediator mediator)
    : IHandler<Guid>
{
    public async Task Handle(Guid id, CancellationToken cancellationToken)
    {
        var imageInfo = await db.Images.FindAsync(id);
        if (imageInfo == null)
            throw new ImageNotFoundException(id);

        var rowsAffected = await db.Images
            .Where(t => t.Id == id && (t.OwnerReference == currentUser.Id || currentUser.IsAdmin))
            .ExecuteDeleteAsync(cancellationToken: cancellationToken);

        if (rowsAffected > 0)
        {
            // Todo: Make this a background job
            photoStore.DeletePhoto(imageInfo.LocalFilePath);
            await mediator.Publish(new ImageDeletedNotification(id), cancellationToken);
        }
        else
            throw new ImageNotFoundException(id);
    }
}