using FotoApi.Features.HandleImages.Exceptions;
using FotoApi.Features.HandleStBilder.Dto;
using FotoApi.Features.HandleStBilder.Exceptions;
using FotoApi.Infrastructure.Repositories;
using FotoApi.Infrastructure.Validation.Exceptions;
using StBildMapper = FotoApi.Features.HandleStBilder.Dto.StBildMapper;

namespace FotoApi.Features.HandleStBilder.Queries;

public class GetStBildQueryHandler : IQueryHandler<GetStBildQuery, StBildResponse>
{
    private readonly PhotoServiceDbContext _db;
    private readonly StBildMapper mapper = new();

    public GetStBildQueryHandler(PhotoServiceDbContext db)
    {
        _db = db;
    }

    public async Task<StBildResponse> Handle(GetStBildQuery request, CancellationToken cancellationToken)
    {
        var stBild = await _db.StBilder.FindAsync(request.Id);

        if (stBild is null)
            throw new StBildNotFoundException(request.Id);

        if (!request.Owner.IsAdmin && stBild.OwnerReference != request.Owner.Id)
            throw new ForbiddenException("User not authorized to get stbild information for this id");
        
        return mapper.ToStBildResponse(stBild);
    }
}