using FotoApi.Abstractions;
using FotoApi.Features.HandleSubmissions.HandleStBilder.Dto;
using FotoApi.Features.HandleSubmissions.HandleStBilder.Exceptions;
using FotoApi.Infrastructure.Repositories.PhotoServiceDbContext;
using FotoApi.Infrastructure.Security.Authorization;
using FotoApi.Infrastructure.Security.Authorization.Policies;
using FotoApi.Infrastructure.Validation.Exceptions;

namespace FotoApi.Features.HandleSubmissions.HandleStBilder.Queries;

public record GetStBildRequest(Guid Id) : ICurrentUser
{
    public CurrentUser CurrentUser { get; set; } = default!;
}

public class GetStBildHandler(PhotoServiceDbContext db, IPolicyChecker policyChecker) : IHandler<GetStBildRequest, StBildResponse>
{
    private readonly StBildResponseMapper _responseMapper = new();

    public async Task<StBildResponse> Handle(GetStBildRequest request, CancellationToken ct)
    {
        var stBild = await db.StBilder.FindAsync(request.Id);

        if (stBild is null)
            throw new StBildNotFoundException(request.Id);
        var isStAdmin = await policyChecker.CompliesToPolicy("StBildAdministratiorPolicy");

        if (!isStAdmin && stBild.OwnerReference != request.CurrentUser.Id)
            throw new UnAuthorizedException("User not authorized to get stbild information for this id");

        return _responseMapper.ToStBildResponse(stBild);
    }
}