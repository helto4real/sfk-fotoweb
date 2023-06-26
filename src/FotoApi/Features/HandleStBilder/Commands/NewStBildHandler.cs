using FotoApi.Features.Shared.Dto;
using FotoApi.Infrastructure.Repositories;
using StBildMapper = FotoApi.Features.HandleStBilder.Dto.StBildMapper;

namespace FotoApi.Features.HandleStBilder.Commands;

public class NewStBildHandler : ICommandHandler<NewStBildCommand, IdentityResponse>
{
    private readonly PhotoServiceDbContext _db;
    private readonly StBildMapper mapper = new();

    public NewStBildHandler(PhotoServiceDbContext db)
    {
        _db = db;
    }

    public async Task<IdentityResponse> Handle(NewStBildCommand request, CancellationToken cancellationToken)
    {
        var stBild = mapper.ToStBild(request);

        await _db.StBilder.AddAsync(
            stBild
        );

        await _db.SaveChangesAsync();

        return new IdentityResponse(stBild.Id);
    }
}