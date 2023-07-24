using FotoApi.Abstractions;
using FotoApi.Features.HandleImages.Exceptions;
using FotoApi.Infrastructure.Repositories;
using FotoApi.Infrastructure.Repositories.PhotoServiceDbContext;
using FotoApi.Infrastructure.Security.Authorization;

namespace FotoApi.Features.HandleStBilder.Commands;

public record DeleteStBildRequest(Guid Id) : ICurrentUser
{
    public CurrentUser CurrentUser { get; set; } = default!;
}

public class DeleteStBildHandler(PhotoServiceDbContext db) : IHandler<DeleteStBildRequest>
{
    public async Task Handle(DeleteStBildRequest request, CancellationToken cancellationToken)
    {
        var imageInfo = await db.StBilder.FindAsync(request.Id);
        if (imageInfo == null)
            throw new ImageNotFoundException(request.Id);

        var rowsAffected = await db.StBilder
            .Where(t => t.Id == request.Id && (t.OwnerReference == request.CurrentUser.Id || request.CurrentUser.IsAdmin))
            .ExecuteDeleteAsync(cancellationToken: cancellationToken);

        if (rowsAffected == 0)
            throw new ImageNotFoundException(request.Id);
    }
}