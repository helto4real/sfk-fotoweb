using FotoApi.Abstractions;
using FotoApi.Features.HandleStBilder.Exceptions;
using FotoApi.Infrastructure.Repositories;
using FotoApi.Infrastructure.Security.Authorization;

namespace FotoApi.Features.HandleStBilder.Commands;

public record AcceptStBildRequest(Guid StBildId, bool StBildAcceptStatus);

public class AcceptStBildHandler(PhotoServiceDbContext db) : IHandler<AcceptStBildRequest>
{
    public async Task Handle(AcceptStBildRequest request, CancellationToken cancellationToken)
    {
        var stBild = await db.StBilder.FindAsync(request.StBildId);
        
        if (stBild == null)
            throw new StBildNotFoundException(request.StBildId);

        if (stBild.IsAccepted != request.StBildAcceptStatus)
        {
            stBild.IsAccepted = request.StBildAcceptStatus;
            db.StBilder.Update(stBild);
            await db.SaveChangesAsync(cancellationToken);
        }
    }
}