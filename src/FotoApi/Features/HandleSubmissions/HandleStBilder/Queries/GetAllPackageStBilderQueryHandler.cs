﻿using FotoApi.Features.HandleSubmissions.HandleStBilder.Dto;
using FotoApi.Infrastructure.Repositories.PhotoServiceDbContext;

namespace FotoApi.Features.HandleSubmissions.HandleStBilder.Queries;

public class GetAllPackageStBilderQueryHandler(PhotoServiceDbContext db) : IEmptyRequestHandler<IReadOnlyCollection<StBildResponse>>
{
    private readonly StBildResponseMapper _responseMapper = new();

    public async Task<IReadOnlyCollection<StBildResponse>> Handle(CancellationToken cancellationToken)
    {
        return await db.StBilder.Where(b => b.IsAccepted && !b.IsUsed)
            .Select(e => _responseMapper.ToStBildResponse(e)).ToListAsync(cancellationToken);
    }
}