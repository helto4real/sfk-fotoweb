using FotoApi.Abstractions;
using FotoApi.Features.HandleStBilder.Dto;
using FotoApi.Infrastructure.Repositories;
using FotoApi.Infrastructure.Repositories.PhotoServiceDbContext;
using FotoApi.Infrastructure.Security.Authorization;

namespace FotoApi.Features.HandleStBilder.Queries;
public record GetAllStBilderForCurrentUserRequest(bool ShowPackagedImages) : ICurrentUser
{
    public CurrentUser CurrentUser { get; set; } = default!;
}

public class GetAllStBilderForCurrentUserHandler(PhotoServiceDbContext db) : IHandler<GetAllStBilderForCurrentUserRequest, List<StBildResponse>>
{
    private readonly StBildResponseMapper _responseMapper = new();

    public async Task<List<StBildResponse>> Handle(GetAllStBilderForCurrentUserRequest request, CancellationToken cancellationToken)
    {
        return request.ShowPackagedImages ? 
            await db.StBilder
                .Where(e => e.OwnerReference == request.CurrentUser.Id).Select(e => _responseMapper.ToStBildResponse(e))
                .ToListAsync(cancellationToken: cancellationToken) :
            await db.StBilder
                .Where(e => e.OwnerReference == request.CurrentUser.Id && e.IsUsed == false).Select(e => _responseMapper.ToStBildResponse(e))
                .ToListAsync(cancellationToken: cancellationToken);
    }
}