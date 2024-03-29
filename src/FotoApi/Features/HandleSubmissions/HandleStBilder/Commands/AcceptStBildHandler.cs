﻿using FotoApi.Features.HandleSubmissions.HandleStBilder.Exceptions;
using FotoApi.Infrastructure.Repositories.PhotoServiceDbContext;

namespace FotoApi.Features.HandleSubmissions.HandleStBilder.Commands;

public record AcceptStBildRequest(Guid StBildId, bool StBildAcceptStatus);

public class AcceptStBildHandler(PhotoServiceDbContext db) : IHandler<AcceptStBildRequest>
{
    public async Task Handle(AcceptStBildRequest request, CancellationToken ct)
    {
        var stBild = await db.StBilder.FindAsync(new object?[] { request.StBildId }, cancellationToken: ct);

        if (stBild == null)
            throw new StBildNotFoundException(request.StBildId);

        if (stBild.IsAccepted != request.StBildAcceptStatus)
        {
            stBild.IsAccepted = request.StBildAcceptStatus;
            db.StBilder.Update(stBild);
            await db.SaveChangesAsync(ct);
        }
    }
}