using System.Net;
using System.Net.Http.Json;
using Foto.Web.Client.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Todo.Web.Shared;

namespace Foto.Web.Client.ApiClients;

public class ImagesApiClient : ApiClientBase
{
    private readonly HttpClient _client;

    public ImagesApiClient(
        HttpClient client,
        AuthorizationManager authManager,
        NavigationManager navigationManager) : base(authManager, navigationManager)
    {
        _client = client;
    }

    public int MaxAllowedImageSize => 1024 * 1024 * 100;

    public async Task<ImageItem?> UploadImage(IBrowserFile? file, string title)
    {
        if (file is null) return null;
    
        var content = new MultipartFormDataContent();
    
        content.Add(new StringContent(file.Name), "filename");
        content.Add(new StringContent(title), "title");
        content.Add(new StreamContent(file.OpenReadStream(MaxAllowedImageSize)), "file", file.Name);
        var response = await _client.PostAsync("api/images", content);
    
        var result = await HandleResponse(response);
        // Todo handle error
        if (result is not null) return null;
        
        return await response.Content.ReadFromJsonAsync<ImageItem>();
    }
    
    public async Task<(ImageItem?, ErrorDetail?)> UploadImageWithMetadata<T>(IBrowserFile? file, string title, T metadata, string metadataType) where T : class
    {
        if (file is null) return (null, new ErrorDetail(){Title = "Du har inte valt en fil", Detail = "Något är fel, försök igen."});

        var content = new MultipartFormDataContent();
        
        content.Add(new StringContent(metadataType), "metadataType");
        content.Add(JsonContent.Create<T>(metadata), "metadata");
        content.Add(new StringContent(title), "title");
        content.Add(new StringContent(file.Name), "filename");
        content.Add(new StreamContent(file.OpenReadStream(MaxAllowedImageSize)), "file", file.Name);
        var response = await _client.PostAsync("api/images", content);

        var result = await HandleResponse(response);

        if (result is not null)
        {
            return (null, result);
        }

        var imageItem = await response.Content.ReadFromJsonAsync<ImageItem>();
        return (imageItem, null);
    }

    public async Task<IEnumerable<ImageItem>?> GetImagesForUserAsync()
    {
        var response = await _client.GetAsync("api/images/user");

        var result = await HandleResponse(response);
        // Todo handle error
        if (result is not null) return null;
        
        return await response.Content.ReadFromJsonAsync<IEnumerable<ImageItem>>();
    }

    public async Task DeleteImageAsync(Guid id)
    {
        var response = await _client.DeleteAsync($"api/images/{id}");
        var result = await HandleResponse(response);
        // Todo handle error
    }
}