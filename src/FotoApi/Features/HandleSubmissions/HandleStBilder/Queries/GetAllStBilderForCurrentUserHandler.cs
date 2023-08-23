using FotoApi.Abstractions;
using FotoApi.Features.HandleSubmissions.HandleStBilder.Dto;
using FotoApi.Infrastructure.Repositories.PhotoServiceDbContext;
using FotoApi.Infrastructure.Security.Authorization;

namespace FotoApi.Features.HandleSubmissions.HandleStBilder.Queries;

public record GetAllStBilderForCurrentUserRequest(bool ShowPackagedImages) : ICurrentUser
{
    public CurrentUser CurrentUser { get; set; } = default!;
}

public class GetAllStBilderForCurrentUserHandler
    (PhotoServiceDbContext db) : IHandler<GetAllStBilderForCurrentUserRequest, IReadOnlyCollection<StBildResponse>>
{
    private readonly StBildResponseMapper _responseMapper = new();

    public async Task<IReadOnlyCollection<StBildResponse>> Handle(GetAllStBilderForCurrentUserRequest request, CancellationToken ct)
    {
        return request.ShowPackagedImages
            ? await db.StBilder
                .Where(e => e.OwnerReference == request.CurrentUser.Id).Select(e => _responseMapper.ToStBildResponse(e))
                .ToListAsync(ct)
            : await db.StBilder
                .Where(e => e.OwnerReference == request.CurrentUser.Id && e.IsUsed == false)
                .Select(e => _responseMapper.ToStBildResponse(e))
                .ToListAsync(ct);
    }
}