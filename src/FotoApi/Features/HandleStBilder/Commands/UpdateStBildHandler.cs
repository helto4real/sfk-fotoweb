using FotoApi.Features.HandleStBilder.Dto;
using FotoApi.Features.HandleStBilder.Exceptions;
using FotoApi.Infrastructure.Repositories;

namespace FotoApi.Features.HandleStBilder.Commands;

public class UpdateStBildHandler : ICommandHandler<UpdateStBildCommand>
{
    private readonly PhotoServiceDbContext _db;
    public UpdateStBildHandler(PhotoServiceDbContext db)
    {
        _db = db;
    }
    public async Task Handle(UpdateStBildCommand request, CancellationToken cancellationToken)
    {
        var stBild = await _db.StBilder.FindAsync(request.Id);
        if (stBild == null)
        {
            throw new StBildNotFoundException(request.Id);
        }
        stBild.Title = request.Title;
        stBild.Name = request.Name;
        stBild.Location = request.Location;
        stBild.Time = request.Time;
        stBild.Description = request.Description;
        stBild.AboutThePhotograper = request.AboutThePhotograper;
        stBild.IsAccepted = request.IsAccepted;
        
        await _db.SaveChangesAsync(cancellationToken);
    }
}