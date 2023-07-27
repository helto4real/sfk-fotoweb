using FotoApi.Features.HandleSubmissions.HandleStBilder.Dto;
using FotoApi.Features.Shared.Dto;
using FotoApi.Infrastructure.Repositories.PhotoServiceDbContext;

namespace FotoApi.Features.HandleSubmissions.HandleStBilder.Commands;

public class NewStBildHandler(PhotoServiceDbContext db) : IHandler<NewStBildRequest, IdentityResponse>
{
    private readonly StBildResponseMapper _responseMapper = new();

    public async Task<IdentityResponse> Handle(NewStBildRequest request, CancellationToken cancellationToken)
    {
        var stBild = _responseMapper.ToStBild(request);
        await db.StBilder.AddAsync(
            stBild
        );

        await db.SaveChangesAsync();

        return new IdentityResponse(stBild.Id);
    }
}