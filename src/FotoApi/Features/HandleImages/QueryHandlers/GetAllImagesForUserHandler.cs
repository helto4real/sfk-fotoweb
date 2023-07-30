using FotoApi.Features.HandleImages.Dto;
using FotoApi.Infrastructure.Repositories.PhotoServiceDbContext;
using FotoApi.Infrastructure.Security.Authorization;

namespace FotoApi.Features.HandleImages.QueryHandlers;

public class GetAllImagesForUserHandler(PhotoServiceDbContext db, CurrentUser currentUser) : IEmptyRequestHandler<List<ImageResponse>>
{
    private readonly ImagesMapper _mapper = new();

    public async Task<List<ImageResponse>> Handle(CancellationToken cancellationToken)
    {
        return await db.Images.Where(todo => todo.OwnerReference == currentUser.Id)
            .Select(t => _mapper.ToImageResponse(t))
            .AsNoTracking().ToListAsync(cancellationToken: cancellationToken);
    }
}