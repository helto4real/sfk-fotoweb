using Riok.Mapperly.Abstractions;
using Image = FotoApi.Model.Image;

namespace FotoApi.Features.HandleImages.Dto;

[Mapper]
public partial class ImagesMapper
{
    public partial ImageResponse ToImageResponse(Image image);
}