using Foto.WebServer.Dto;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Options;

namespace Foto.WebServer.Services;

public class ImageService : IImageService
{
    private readonly HttpClient _httpClient;

    public ImageService(
        HttpClient httpClient, IOptions<AppSettings> appSettings)
    {
        _httpClient = httpClient;
        httpClient.BaseAddress = new Uri(appSettings.Value.FotoApiUrl);
        httpClient.DefaultRequestHeaders.Add("User-Agent", "FotoWebbServer");
    }

    public async Task<(ImageItem?, ErrorDetail?)> UploadImageWithMetadata<T>(IBrowserFile? file, string title,
        T metadata, string metadataType) where T : class
    {
        if (file is null)
            return (null, new ErrorDetail { Title = "Du har inte valt en fil", Detail = "Något är fel, försök igen." });

        var content = new MultipartFormDataContent();

        content.Add(new StringContent(metadataType), "metadataType");
        content.Add(JsonContent.Create(metadata), "metadata");
        content.Add(new StringContent(title), "title");
        content.Add(new StringContent(file.Name), "filename");
        content.Add(new StreamContent(file.OpenReadStream(IImageService.MaxAllowedImageSize)), "file", file.Name);
        var response = await _httpClient.PostAsync("api/images", content);

        var result = await HandleResponse(response);

        if (result is not null) return (null, result);

        var imageItem = await response.Content.ReadFromJsonAsync<ImageItem>();
        return (imageItem, null);
    }

    public async Task<(ImageItem?, ErrorDetail?)> UploadImage(IBrowserFile? file, string title)
    {
        if (file is null)
            return (null, new ErrorDetail { Title = "Du har inte valt en fil", Detail = "Något är fel, försök igen." });

        var content = new MultipartFormDataContent();

        content.Add(new StringContent(file.Name), "filename");
        content.Add(new StringContent(title), "title");
        content.Add(new StreamContent(file.OpenReadStream(IImageService.MaxAllowedImageSize)), "file", file.Name);
        var response = await _httpClient.PostAsync("api/images", content);

        var result = await HandleResponse(response);
        if (result is not null) return (null, result);

        var imageItem = await response.Content.ReadFromJsonAsync<ImageItem>();

        return (imageItem, null);
    }

    public async Task<IEnumerable<ImageItem>> GetImagesForUserAsync()
    {
        var response = await _httpClient.GetAsync("api/images/user");

        var result = await HandleResponse(response);
        // Todo handle error
        if (result is not null) return Enumerable.Empty<ImageItem>();

        return await response.Content.ReadFromJsonAsync<IEnumerable<ImageItem>>() ?? Enumerable.Empty<ImageItem>();
    }

    public async Task DeleteImageAsync(Guid id)
    {
        await _httpClient.DeleteAsync($"api/images/{id}");
    }

    public async Task<ErrorDetail?> HandleResponse(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode) return await response.Content.ReadFromJsonAsync<ErrorDetail>();

        return null;
    }
}