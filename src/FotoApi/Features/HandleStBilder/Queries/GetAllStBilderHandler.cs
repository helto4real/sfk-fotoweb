﻿using FotoApi.Features.HandleStBilder.Dto;
using FotoApi.Infrastructure.Repositories;
using StBildMapper = FotoApi.Features.HandleStBilder.Dto.StBildMapper;

namespace FotoApi.Features.HandleStBilder.Queries;

public class GetAllStBilderHandler : IQueryHandler<GetAllStBilderQuery, List<StBildResponse>>
{
    private readonly PhotoServiceDbContext _db;
    private readonly StBildMapper _mapper = new();
    public GetAllStBilderHandler(PhotoServiceDbContext db)
    {
        _db = db;
    }
    public async Task<List<StBildResponse>> Handle(GetAllStBilderQuery request, CancellationToken cancellationToken)
    {
        return request.ShowPackagedImages ? 
            await _db.StBilder
                .Select(e => _mapper.ToStBildResponse(e))
                .ToListAsync(cancellationToken: cancellationToken) :
            await _db.StBilder
                .Where(e=> e.IsUsed == false).Select(e => _mapper.ToStBildResponse(e))
                .ToListAsync(cancellationToken: cancellationToken);
    }
}