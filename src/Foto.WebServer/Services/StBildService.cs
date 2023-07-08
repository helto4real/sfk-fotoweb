using System.Net.Http.Headers;
using Blazored.LocalStorage;
using Foto.WebServer.Dto;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Foto.WebServer.Services;

public class StBildService : IStBildService
{
    private readonly AppSettings _appSettings;

    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _accessor;

    public StBildService(HttpClient httpClient, IOptions<AppSettings> appSettings,
        IHttpContextAccessor accessor)
    {
        _httpClient = httpClient;
        _accessor = accessor;
        _appSettings = appSettings.Value;
        httpClient.BaseAddress = new Uri(_appSettings.FotoApiUrl);
        httpClient.DefaultRequestHeaders.Add("User-Agent", "FotoWebbServer");
    }

    public async Task<StBildInfo?> GetStBildAsync(Guid stbildId)
    {
        await AddAuthorizationHeaders();
        var response = await _httpClient.GetAsync($"/api/stbilder/{stbildId}");
        if (response.IsSuccessStatusCode)
        {
            var stBild = await response.Content.ReadFromJsonAsync<StBildInfo>();
            return stBild;
        }

        return null;
    }

    public async Task UpdateStBildAsync(StBildInfo stBild)
    {
        await AddAuthorizationHeaders();
        await _httpClient.PutAsJsonAsync($"/api/stbilder/{stBild.Id}", stBild);
    }

    public async Task<List<StBildInfo>> GetStBilder(bool showPackagedImages)
    {
        await AddAuthorizationHeaders();
        var response = await _httpClient.GetAsync($"/api/stbilder?showPackagedImages={showPackagedImages}");
        if (response.IsSuccessStatusCode)
        {
            var stBilder = await response.Content.ReadFromJsonAsync<List<StBildInfo>>();
            if (stBilder is not null)
                return stBilder;
        }

        return new List<StBildInfo>();
    }

    public async Task<List<StBildInfo>> GetApprovedNotPackagedStBilderAsync()
    {
        await AddAuthorizationHeaders();
        var response = await _httpClient.GetAsync($"/api/stbilder/packageble");
        if (response.IsSuccessStatusCode)
        {
            var stBilder = await response.Content.ReadFromJsonAsync<List<StBildInfo>>();
            if (stBilder is not null)
                return stBilder;
        }

        return new List<StBildInfo>();
    }

    public async Task<bool> PackageStBilder(GuidIds guidIds)
    {
        await AddAuthorizationHeaders();
        var response = await _httpClient.PostAsJsonAsync($"/api/stbilder/package", guidIds);
        return response.IsSuccessStatusCode;
    }

    private async Task AddAuthorizationHeaders()
    {
        var authResult = await _accessor.HttpContext!.AuthenticateAsync();
        var properties = authResult.Properties!;

        var token = properties.GetTokenValue(TokenNames.AccessToken);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}