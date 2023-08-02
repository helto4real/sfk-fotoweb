using FotoApi.Features.HandleSubmissions.HandleStBilder.Dto;
using FotoApi.Features.HandleSubmissions.HandleStBilder.Exceptions;
using FotoApi.Infrastructure.Repositories.PhotoServiceDbContext;

namespace FotoApi.Features.HandleSubmissions.HandleStBilder.Commands;

public class UpdateStBildHandler(PhotoServiceDbContext db) : IHandler<StBildRequest>
{
    public async Task Handle(StBildRequest request, CancellationToken ct)
    {
        var stBild = await db.StBilder.FindAsync(request.Id);
        if (stBild == null) throw new StBildNotFoundException(request.Id);
        stBild.Title = request.Title;
        stBild.Name = request.Name;
        stBild.Location = request.Location;
        stBild.Time = request.Time;
        stBild.Description = request.Description;
        stBild.AboutThePhotographer = request.AboutThePhotographer;
        stBild.IsAccepted = request.IsAccepted;

        await db.SaveChangesAsync(ct);
    }
}