using FotoApi.Abstractions;
using FotoApi.Features.HandleImages.Dto;
using FotoApi.Features.HandleImages.Exceptions;
using FotoApi.Infrastructure.Repositories.PhotoServiceDbContext;
using FotoApi.Infrastructure.Security.Authorization;
using Wolverine;

namespace FotoApi.Features.HandleSubmissions.HandleStBilder.Commands;

public record DeleteStBildRequest(Guid Id) : ICurrentUser
{
    public CurrentUser CurrentUser { get; set; } = default!;
}

public class DeleteStBildHandler(PhotoServiceDbContext db, IMessageBus bus) : IHandler<DeleteStBildRequest>
{
    public async Task Handle(DeleteStBildRequest request, CancellationToken ct)
    {
        var imageInfo = await db.StBilder.FindAsync(new object?[] { request.Id }, cancellationToken: ct);
        if (imageInfo == null)
            throw new ImageNotFoundException(request.Id);

        var rowsAffected = await db.StBilder
            .Where(t => t.Id == request.Id &&
                        (t.OwnerReference == request.CurrentUser.Id || request.CurrentUser.IsAdmin))
            .ExecuteDeleteAsync(ct);

        if (rowsAffected == 0)
            throw new ImageNotFoundException(request.Id);

        await bus.PublishAsync(new DeleteImageCommandNotification(imageInfo.ImageReference));
    }
}