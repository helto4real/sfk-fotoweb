using FotoApi.Abstractions;
using FotoApi.Features.HandleSubmissions.HandleStBilder.Dto;
using FotoApi.Features.HandleSubmissions.HandleStBilder.Exceptions;
using FotoApi.Infrastructure.Repositories.PhotoServiceDbContext;
using FotoApi.Infrastructure.Security.Authorization;
using FotoApi.Infrastructure.Validation.Exceptions;

namespace FotoApi.Features.HandleSubmissions.HandleStBilder.Queries;

public record GetStBildRequest(Guid Id) : ICurrentUser
{
    public CurrentUser CurrentUser { get; set; } = default!;
}

public class GetStBildHandler(PhotoServiceDbContext db) : IHandler<GetStBildRequest, StBildResponse>
{
    private readonly StBildResponseMapper _responseMapper = new();

    public async Task<StBildResponse> Handle(GetStBildRequest request, CancellationToken cancellationToken)
    {
        var stBild = await db.StBilder.FindAsync(request.Id);

        if (stBild is null)
            throw new StBildNotFoundException(request.Id);

        if (!request.CurrentUser.IsAdmin && stBild.OwnerReference != request.CurrentUser.Id)
            throw new ForbiddenException("User not authorized to get stbild information for this id");
        
        return _responseMapper.ToStBildResponse(stBild);
    }
}