using Foto.WebServer.Dto;
using Microsoft.Extensions.Options;

namespace Foto.WebServer.Services;

public class StBildService : ServiceBase, IStBildService
{
    private readonly IHttpContextAccessor _accessor;

    private readonly HttpClient _httpClient;
    private readonly ISignInService _signInService;

    public StBildService(HttpClient httpClient, IOptions<AppSettings> appSettings,
        IHttpContextAccessor accessor, ISignInService signInService, ILogger<StBildService> logger) : base(logger)
    {
        _httpClient = httpClient;
        _accessor = accessor;
        _signInService = signInService;
        httpClient.BaseAddress = new Uri(appSettings.Value.FotoApiUrl);
        httpClient.DefaultRequestHeaders.Add("User-Agent", "FotoWebbServer");
    }

    public async Task<(StBildInfo?, ErrorDetail?)> GetStBildAsync(Guid stbildId)
    {
        var response =
            await _signInService.RefreshTokenOnExpired(async () =>
                await _httpClient.GetAsync($"/api/stbilder/{stbildId}"));
        
        var result = await HandleResponse(response);
        if (result is not null) return (null, result);
        
        var stBild = await response.Content.ReadFromJsonAsync<StBildInfo>();
        stBild!.Time = stBild.Time.ToLocalTime();
        return (stBild, null);
    }

    public async Task UpdateStBildAsync(StBildInfo stBild)
    {
        stBild.Time = stBild.Time.ToUniversalTime();
        await _signInService.RefreshTokenOnExpired(async () =>
            await _httpClient.PutAsJsonAsync($"/api/stbilder/{stBild.Id}", stBild));
    }

    public async Task<List<StBildInfo>> GetStBilderForCurrentUser(bool showPackagedImages)
    {
        var response = await _signInService.RefreshTokenOnExpired(async () =>
            await _httpClient.GetAsync($"/api/stbilder/user/{showPackagedImages}"));
        if (response.IsSuccessStatusCode)
        {
            var stBilder = await response.Content.ReadFromJsonAsync<List<StBildInfo>>();
            if (stBilder is not null)
            {
                foreach (var stBildInfo in stBilder) stBildInfo.Time = stBildInfo.Time.ToLocalTime();

                return stBilder;
            }
        }

        return new List<StBildInfo>();
    }

    public async Task<List<StBildInfo>> GetStBilder(bool showPackagedImages)
    {
        var response = await _signInService.RefreshTokenOnExpired(async () =>
            await _httpClient.GetAsync($"/api/stbilder/{showPackagedImages}"));
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
        var response =
            await _signInService.RefreshTokenOnExpired(async () =>
                await _httpClient.GetAsync("/api/stbilder/packageble"));
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
        var response = await _signInService.RefreshTokenOnExpired(async () =>
            await _httpClient.PostAsJsonAsync("/api/stbilder/package", guidIds));
        return response.IsSuccessStatusCode;
    }

    public async Task<ErrorDetail?> SetAcceptStatusForStBild(Guid stBildId, bool stBildIsAccepted)
    {
        var response = await _signInService.RefreshTokenOnExpired(async () =>
            await _httpClient.PostAsync($"/api/stbilder/{stBildId}/acceptstatus/{stBildIsAccepted}", null));
        var result = await HandleResponse(response);
        return result ?? null;
    }

    public async Task<(List<StBildPackageInfo>?, ErrorDetail?)> GetStBildPackagesAsync(bool returnDelivered)
    {
        var response = await _signInService.RefreshTokenOnExpired(async () =>
            await _httpClient.GetAsync($"/api/stbilder/packages/{returnDelivered}"));
        var result = await HandleResponse(response);
        if (result is not null) return (null, result);
        var packages = await response.Content.ReadFromJsonAsync<List<StBildPackageInfo>>();
        return (packages, null);
    }

    public async Task<ErrorDetail?> SetPackageStatusDelivered(Guid id)
    {
        var response = await _signInService.RefreshTokenOnExpired(async () =>
            await _httpClient.GetAsync($"/api/stbilder/stpackage/set-delivered/{id}"));
        var result = await HandleResponse(response);
        return result;
    }
}