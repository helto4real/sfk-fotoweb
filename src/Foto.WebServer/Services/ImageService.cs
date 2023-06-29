using System.Net.Http.Headers;
using Blazored.LocalStorage;
using Foto.WebServer.Dto;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Options;

namespace Foto.WebServer.Services;

public class ImageService : IImageService
{
    private readonly HttpClient _httpClient;
    //Todo: refactor to use the AuthenticationStateProvider
    private readonly ILocalStorageService _localStorageService;
    private readonly HttpHelper _httpHelper;
    private readonly AppSettings _appSettings;
    public ImageService(
        HttpClient httpClient, IOptions<AppSettings> appSettings,
        ILocalStorageService localStorageService, HttpHelper httpHelper)
    {
        _httpClient = httpClient;
        _localStorageService = localStorageService;
        _httpHelper = httpHelper;
        _appSettings = appSettings.Value;
        httpClient.BaseAddress = new Uri(_appSettings.FotoApiUrl);
        httpClient.DefaultRequestHeaders.Add("User-Agent", "FotoWebbServer");
    }

    public async Task<(ImageItem?, ErrorDetail?)> UploadImageWithMetadata<T>(IBrowserFile? file, string title, T metadata, string metadataType) where T : class
    {
        await AddAuthorizationHeaders();
        
        if (file is null) return (null, new ErrorDetail(){Title = "Du har inte valt en fil", Detail = "Något är fel, försök igen."});

        var content = new MultipartFormDataContent();
        
        content.Add(new StringContent(metadataType), "metadataType");
        content.Add(JsonContent.Create<T>(metadata), "metadata");
        content.Add(new StringContent(title), "title");
        content.Add(new StringContent(file.Name), "filename");
        content.Add(new StreamContent(file.OpenReadStream(IImageService.MaxAllowedImageSize)), "file", file.Name);
        var response = await _httpClient.PostAsync("api/images", content);

        var result = await _httpHelper.HandleResponse(response);

        if (result is not null)
        {
            return (null, result);
        }

        var imageItem = await response.Content.ReadFromJsonAsync<ImageItem>();
        return (imageItem, null);
    }

    public async Task<(ImageItem?, ErrorDetail?)> UploadImage(IBrowserFile? file, string title)
    {
        if (file is null) return (null, new ErrorDetail(){Title = "Du har inte valt en fil", Detail = "Något är fel, försök igen."});

        await AddAuthorizationHeaders();
    
        var content = new MultipartFormDataContent();
    
        content.Add(new StringContent(file.Name), "filename");
        content.Add(new StringContent(title), "title");
        content.Add(new StreamContent(file.OpenReadStream(IImageService.MaxAllowedImageSize)), "file", file.Name);
        var response = await _httpClient.PostAsync("api/images", content);
    
        var result = await _httpHelper.HandleResponse(response);
        if (result is not null)
        {
            return (null, result);
        }
        
        var imageItem = await response.Content.ReadFromJsonAsync<ImageItem>();
        
        return (imageItem, null);
    }

    public async Task<IEnumerable<ImageItem>> GetImagesForUserAsync()
    {
        await AddAuthorizationHeaders();
        var response = await _httpClient.GetAsync("api/images/user");

        var result = await _httpHelper.HandleResponse(response);
        // Todo handle error
        if (result is not null) return Enumerable.Empty<ImageItem>();
        
        return await response.Content.ReadFromJsonAsync<IEnumerable<ImageItem>>() ?? Enumerable.Empty<ImageItem>();
    }

    public async Task DeleteImageAsync(Guid id)
    {
        await AddAuthorizationHeaders();
        await _httpClient.DeleteAsync($"api/images/{id}");
    }

    private async Task AddAuthorizationHeaders()
    {
        var token = await _localStorageService.GetItemAsync<string>("accessToken");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}