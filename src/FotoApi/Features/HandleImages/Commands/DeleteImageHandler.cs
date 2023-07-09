using FotoApi.Features.HandleImages.Dto;
using FotoApi.Features.HandleImages.Exceptions;
using FotoApi.Infrastructure.Repositories;
using FotoApi.Infrastructure.Security.Authorization;
using MediatR;

namespace FotoApi.Features.HandleImages.Commands;

public class DeleteImageHandler : ICommandHandler<DeleteImageCommand>
{
    private readonly PhotoServiceDbContext _db;
    private readonly IMediator _mediator;
    private readonly IPhotoStore _photoStore;

    public DeleteImageHandler(PhotoServiceDbContext db, IPhotoStore photoStore, CurrentUser owner, IMediator mediator)
    {
        _db = db;
        _photoStore = photoStore;
        _mediator = mediator;
    }

    public async Task Handle(DeleteImageCommand request, CancellationToken cancellationToken)
    {
        var imageInfo = await _db.Images.FindAsync(request.Id);
        if (imageInfo == null)
            throw new ImageNotFoundException(request.Id);

        var rowsAffected = await _db.Images
            .Where(t => t.Id == request.Id && (t.OwnerReference == request.Owner.Id || request.Owner.IsAdmin))
            .ExecuteDeleteAsync();

        if (rowsAffected > 0)
        {
            _photoStore.DeletePhoto(imageInfo.LocalFilePath);
            await _mediator.Publish(new ImageDeletedNotification(request.Id), cancellationToken);
        }
        else
            throw new ImageNotFoundException(request.Id);
    }
}