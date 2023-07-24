using Foto.WebServer.Dto;
using Microsoft.Extensions.Options;

namespace Foto.WebServer.Services;

public class UserService : IUserService
{
    private readonly HttpClient _httpClient;

    public UserService(HttpClient httpClient, IOptions<AppSettings> appSettings)
    {
        _httpClient = httpClient;
        _appSettings = appSettings.Value;
        httpClient.BaseAddress = new Uri(_appSettings.FotoApiUrl);
        httpClient.DefaultRequestHeaders.Add("User-Agent", "FotoWebbServer");
    }

    private AppSettings _appSettings { get; }

    public async Task<bool> ConfirmEmailAsync(string token)
    {
        var escapedToken = Uri.EscapeDataString(token);
        var result = await _httpClient.GetAsync($@"api/public/confirmemail/{escapedToken}");
        return result.IsSuccessStatusCode;
    }

    public async Task<ErrorDetail?> HandleResponse(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode) return await response.Content.ReadFromJsonAsync<ErrorDetail>();

        return null;
    }
}