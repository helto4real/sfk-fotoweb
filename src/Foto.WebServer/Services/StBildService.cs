using System.Net.Http.Headers;
using Blazored.LocalStorage;
using Foto.WebServer.Dto;
using Microsoft.Extensions.Options;

namespace Foto.WebServer.Services;

public class StBildService : IStBildService
{
    private readonly AppSettings _appSettings;

    private readonly HttpClient _httpClient;

    //Todo: refactor to use the AuthenticationStateProvider
    private readonly ILocalStorageService _localStorageService;

    public StBildService(HttpClient httpClient, IOptions<AppSettings> appSettings,
        ILocalStorageService localStorageService)
    {
        _httpClient = httpClient;
        _localStorageService = localStorageService;
        _appSettings = appSettings.Value;
        httpClient.BaseAddress = new Uri(_appSettings.FotoApiUrl);
        httpClient.DefaultRequestHeaders.Add("User-Agent", "FotoWebbServer");
    }

    public async Task<StBildInfo?> GetStBild(Guid stbildId)
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

    public async Task UpdateStBild(StBildInfo stBild)
    {
        await AddAuthorizationHeaders();
        await _httpClient.PutAsJsonAsync($"/api/stbilder/{stBild.Id}", stBild);
    }

    public async Task<List<StBildInfo>> GetStBilder(bool useMyImages)
    {
        await AddAuthorizationHeaders();
        var response = await _httpClient.GetAsync($"/api/stbilder?useMyImages={useMyImages}");
        if (response.IsSuccessStatusCode)
        {
            var stBilder = await response.Content.ReadFromJsonAsync<List<StBildInfo>>();
            if (stBilder is not null)
                return stBilder;
        }

        return new List<StBildInfo>();
    }

    private async Task AddAuthorizationHeaders()
    {
        var token = await _localStorageService.GetItemAsync<string>("accessToken");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}