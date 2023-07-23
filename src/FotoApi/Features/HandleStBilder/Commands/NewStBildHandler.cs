using FotoApi.Features.HandleStBilder.Dto;
using FotoApi.Features.Shared.Dto;
using FotoApi.Infrastructure.Repositories;
using StBildMapper = FotoApi.Features.HandleStBilder.Dto.StBildMapper;

namespace FotoApi.Features.HandleStBilder.Commands;

public class NewStBildHandler(PhotoServiceDbContext db) : IHandler<NewStBildRequest, IdentityResponse>
{
    private readonly StBildMapper mapper = new();

    public async Task<IdentityResponse> Handle(NewStBildRequest request, CancellationToken cancellationToken)
    {
        var stBild = mapper.ToStBild(request);

        await db.StBilder.AddAsync(
            stBild
        );

        await db.SaveChangesAsync();

        return new IdentityResponse(stBild.Id);
    }
}