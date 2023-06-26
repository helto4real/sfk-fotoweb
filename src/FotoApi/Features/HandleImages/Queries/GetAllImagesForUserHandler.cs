using FotoApi.Features.HandleImages.Dto;
using FotoApi.Infrastructure.Repositories;

namespace FotoApi.Features.HandleImages.Queries;

public class GetAllImagesForUserHandler : IQueryHandler<GetAllImagesForUserQuery, List<ImageResponse>>
{
    private readonly PhotoServiceDbContext _db;
    private readonly ImagesMapper _mapper = new();

    public GetAllImagesForUserHandler(PhotoServiceDbContext db)
    {
        _db = db;
    }

    public async Task<List<ImageResponse>> Handle(GetAllImagesForUserQuery request, CancellationToken cancellationToken)
    {
        return await _db.Images.Where(todo => todo.OwnerReference == request.Owner.Id)
            .Select(t => _mapper.ToImageResponse(t))
            .AsNoTracking().ToListAsync();
    }
}