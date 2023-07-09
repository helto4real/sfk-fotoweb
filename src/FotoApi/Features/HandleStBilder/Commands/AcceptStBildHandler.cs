using FotoApi.Features.HandleStBilder.Exceptions;
using FotoApi.Infrastructure.Repositories;

namespace FotoApi.Features.HandleStBilder.Commands;

public class AcceptStBildHandler : ICommandHandler<AcceptStBildCommand>
{
    private readonly PhotoServiceDbContext _db;

    public AcceptStBildHandler(
        PhotoServiceDbContext db
        )
    {
        _db = db;
    }
    public async Task Handle(AcceptStBildCommand request, CancellationToken cancellationToken)
    {
        var stBild = await _db.StBilder.FindAsync(request.StBildId);
        
        if (stBild == null)
            throw new StBildNotFoundException(request.StBildId);

        if (stBild.IsAccepted != request.StBildAcceptStatus)
        {
            stBild.IsAccepted = request.StBildAcceptStatus;
            _db.StBilder.Update(stBild);
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}