using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using Foto.WebServer.Dto;
using Microsoft.Extensions.Options;

namespace Foto.WebServer.Services;

public class AdminService : IAdminService
{
    private readonly AppSettings _appSettings;
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);
    private readonly ILogger<AdminService> _logger;
    private readonly ISignInService _signInService;

    public AdminService(
        HttpClient httpClient,
        IOptions<AppSettings> appSettings,
        ISignInService signInService,
        ILogger<AdminService> logger)
    {
        _httpClient = httpClient;
        _appSettings = appSettings.Value;
        _signInService = signInService;
        _logger = logger;
        _jsonOptions.Converters.Add(new JsonStringEnumConverter());
        httpClient.BaseAddress = new Uri(_appSettings.FotoApiUrl);
        httpClient.DefaultRequestHeaders.Add("User-Agent", "FotoWebbServer");
    }

 

    public async Task<IEnumerable<UrlToken>> GetCurrentTokens()
    {
        var response =
            await _signInService.RefreshTokenOnExpired(async () =>
                await _httpClient.GetAsync("api/admin/token/valid-tokens"));

        if (!response.IsSuccessStatusCode) return new List<UrlToken>();

        return await response.Content.ReadFromJsonAsync<List<UrlToken>>(_jsonOptions) ?? new List<UrlToken>();
    }

    public async ValueTask DeleteToken(Guid tokenId)
    {
        var response =
            await _signInService.RefreshTokenOnExpired(async () =>
                await _httpClient.DeleteAsync($"api/admin/token/{tokenId}"));
    }

    public async Task<UrlToken?> AddTokenByTokenType(UrlTokenType tokenType)
    {
        var response = await _signInService.RefreshTokenOnExpired(async () =>
            await _httpClient.PostAsJsonAsync("api/admin/token/addtokenbytype",
                new NewTokenByType { UrlTokenType = tokenType }));

        // Todo handle error
        if (!response.IsSuccessStatusCode) return null;

        return await response.Content.ReadFromJsonAsync<UrlToken>(_jsonOptions);
    }

    public async Task<(ReadOnlyCollection<RoleInfo>?, ErrorDetail?)> GetRolesAsync()
    {
        var response = await _signInService.RefreshTokenOnExpired(async () =>
            await _httpClient.GetAsync($"api/admin/roles"));
        var result = await HandleResponse(response);
        if (result is not null) return (null, result);
        var member = await response.Content.ReadFromJsonAsync<List<RoleInfo>>();
        return (member!.AsReadOnly(), null);
    }


    private async Task<ErrorDetail?> HandleResponse(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode) return await response.Content.ReadFromJsonAsync<ErrorDetail>();

        return null;
    }
}