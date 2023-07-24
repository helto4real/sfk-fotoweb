using Foto.WebServer.Dto;
using Microsoft.AspNetCore.Components.Forms;

namespace Foto.WebServer.Services;

public interface IImageService
{
    static int MaxAllowedImageSize => 1024 * 1024 * 10; // 10 MB

    Task<(ImageItem?, ErrorDetail?)> UploadImageWithMetadata<T>(IBrowserFile? file, string title, T metadata,
        string metadataType) where T : class;

    Task<(ImageItem?, ErrorDetail?)> UploadImage(IBrowserFile? imageFile, string title);

    Task<IEnumerable<ImageItem>> GetImagesForUserAsync();
    Task DeleteImageAsync(Guid imageItemId);
}