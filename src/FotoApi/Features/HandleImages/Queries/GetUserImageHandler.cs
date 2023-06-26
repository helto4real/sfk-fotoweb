using FotoApi.Features.HandleImages.Dto;
using FotoApi.Features.HandleImages.Exceptions;
using FotoApi.Infrastructure.Repositories;

namespace FotoApi.Features.HandleImages.Queries;

public class GetUserImageHandler : IQueryHandler<GetUserImageQuery, ImageResponse>
{
    private readonly PhotoServiceDbContext _db;
    private readonly ImagesMapper _mapper = new();

    public GetUserImageHandler(PhotoServiceDbContext db)
    {
        _db = db;
    }

    public async Task<ImageResponse> Handle(GetUserImageQuery query, CancellationToken cancellationToken)
    {
        return await _db.Images.FindAsync(query.Id) switch
        {
            { } image when image.OwnerReference == query.Id.ToString() || query.Owner.IsAdmin =>
                _mapper.ToImageResponse(image),
            _ => throw new ImageNotFoundException(query.Id)
        };
    }
}