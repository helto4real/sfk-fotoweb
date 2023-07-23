﻿using FotoApi.Features.HandleStBilder.Dto;
using FotoApi.Features.HandleStBilder.Exceptions;
using FotoApi.Infrastructure.Repositories;

namespace FotoApi.Features.HandleStBilder.Commands;

public class UpdateStBildHandler(PhotoServiceDbContext db) : IHandler<StBildRequest>
{
    public async Task Handle(StBildRequest request, CancellationToken cancellationToken)
    {
        var stBild = await db.StBilder.FindAsync(request.Id);
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
        
        await db.SaveChangesAsync(cancellationToken);
    }
}