using Foto.WebServer.Dto;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Options;

namespace Foto.WebServer.Services;

public class ImageService : ServiceBase, IImageService 
{
    private readonly HttpClient _httpClient;
    private readonly ISignInService _signInService;

    public ImageService(
        HttpClient httpClient, 
        IOptions<AppSettings> appSettings, 
        ISignInService signInService,
        ILogger<ImageService> logger) : base(logger)
    {
        _httpClient = httpClient;
        _signInService = signInService;
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
        var response =
            await _signInService.RefreshTokenOnExpired(async () =>
                await _httpClient.PostAsync("api/images", content));

        var result = await HandleResponse(response);

        if (result is not null) return (null, result);

        var imageItem = await response.Content.ReadFromJsonAsync<ImageItem>();
        return (imageItem, null);
    }

    public async Task<(ImageItem?, ErrorDetail?)> UploadImage(IBrowserFile? file, string title)
    {
        if (file is null)
            return (null, new ErrorDetail { Title = "Du har inte valt en fil", Detail = "Något är fel, försök igen." });

        // First we validate the access token and have it refreshed if needed before we upload the image
        if (!await _signInService.ValidateAccessTokenAndRefreshIfNeedAsync(_httpClient))
        {
            return (null, new ErrorDetail
            {
                Title = "Din inloggning har gått ut",
                Detail = "Logga in igen för att kunna ladda upp bilder"
            });
        }
        var content = new MultipartFormDataContent();

        var stream = file.OpenReadStream(IImageService.MaxAllowedImageSize);
        var streamContent = new StreamContent(stream);
        content.Add(new StringContent(file.Name), "filename");
        content.Add(new StringContent(title), "title");
        content.Add(streamContent, "file", file.Name);
        
        var response =  await _signInService.RefreshTokenOnExpired(async () =>
            await _httpClient.PostAsync("api/images", content));

        var result = await HandleResponse(response);
        if (result is not null) return (null, result);

        var imageItem = await response.Content.ReadFromJsonAsync<ImageItem>();

        return (imageItem, null);
    }

    public async Task<IEnumerable<ImageItem>> GetImagesForUserAsync()
    {
        var response =
            await _signInService.RefreshTokenOnExpired(async () => 
                await _httpClient.GetAsync("api/images/user"));
        var result = await HandleResponse(response);
        // Todo handle error
        if (result is not null) return Enumerable.Empty<ImageItem>();

        return await response.Content.ReadFromJsonAsync<IEnumerable<ImageItem>>() ?? Enumerable.Empty<ImageItem>();
    }

    public async Task DeleteImageAsync(Guid id)
    {
        await _signInService.RefreshTokenOnExpired(async () =>
            await _httpClient.DeleteAsync($"api/images/{id}"));
    }
}