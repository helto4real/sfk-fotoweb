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
        await _httpClient.PutAsJsonAsync($"/api/stbilder/{stBild.Id}", stBild);
    }

    public async Task<List<StBildInfo>> GetStBilderForCurrentUser(bool showPackagedImages)
    {
        var response = await _httpClient.GetAsync($"/api/stbilder/{showPackagedImages}");
        if (response.IsSuccessStatusCode)
        {
            var stBilder = await response.Content.ReadFromJsonAsync<List<StBildInfo>>();
            if (stBilder is not null)
                return stBilder;
        }

        return new List<StBildInfo>();
    }
    
    public async Task<List<StBildInfo>> GetStBilder(bool showPackagedImages)
    {
        var response = await _httpClient.GetAsync($"/api/stbilder/all/{showPackagedImages}");
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
        var response = await _httpClient.PostAsJsonAsync($"/api/stbilder/package", guidIds);
        return response.IsSuccessStatusCode;
    }

    public async Task<ErrorDetail?> SetAcceptStatusForStBild(Guid stBildId, bool stBildIsAccepted)
    {
        var response = await _httpClient.PostAsync($"/api/stbilder/{stBildId}/acceptstatus/{stBildIsAccepted}", null);
        var result = await HandleResponse(response);
        return result ?? null;
    }
    private async Task<ErrorDetail?> HandleResponse(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<ErrorDetail>();
        }

        return null;
    }
}