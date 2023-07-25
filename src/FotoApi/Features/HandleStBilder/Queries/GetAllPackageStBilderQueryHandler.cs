﻿using FotoApi.Features.HandleStBilder.Dto;
using FotoApi.Infrastructure.Repositories;
using FotoApi.Infrastructure.Repositories.PhotoServiceDbContext;

namespace FotoApi.Features.HandleStBilder.Queries;

public class GetAllPackageStBilderQueryHandler(PhotoServiceDbContext db) : IEmptyRequestHandler<List<StBildResponse>>
{
    private readonly StBildResponseMapper _responseMapper = new();

    public async Task<List<StBildResponse>> Handle(CancellationToken cancellationToken)
    {
        return await db.StBilder.Where(b => b.IsAccepted && !b.IsUsed)
            .Select(e => _responseMapper.ToStBildResponse(e)).ToListAsync(cancellationToken: cancellationToken);
    }
}