using System.Net;
using Foto.WebServer.Dto;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Foto.WebServer.Services;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly IOptions<AppSettings> _appSettings;
    private readonly IMemoryCache _cache;

    public AuthService(
        HttpClient httpClient, 
        IOptions<AppSettings> appSettings,
        IMemoryCache cache
        )
    {
        _httpClient = httpClient;
        _appSettings = appSettings;
        _cache = cache;
        
        httpClient.BaseAddress = new Uri(_appSettings.Value.FotoApiUrl);
        httpClient.DefaultRequestHeaders.Add("User-Agent", "FotoWebbServer");
    }
    
    public async Task<(LoginInfo?, ErrorDetail?)> LoginAsync(LoginUserInfo loginInfo)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/users/token", loginInfo);
        var result = await HandleResponse(response);
        if (result is not null)
        {
            return (null, result);
        }
       
        var loginInfoResponse = await response.Content.ReadFromJsonAsync<LoginInfo>();
        if (loginInfoResponse is null) return (null, new ErrorDetail(){Title = "Internt fel", Detail = "Försök att logga in igen eller kontakta administratör vid återkommande problem"});

        TimeSpan duration = loginInfoResponse.RefreshTokenExpiration - DateTime.UtcNow;
        
        // We cache the access token with the refresh token as key so we do not have to call the API for every request
        _cache.Set(loginInfoResponse.RefreshToken, loginInfoResponse.Token, duration);
        return (loginInfoResponse, null);
    }
    
    public async Task<LoginInfo?> GetOrRegisterUserAsync(string provider, ExternalUserInfo userInfo)
    {
        var response = await _httpClient.PostAsJsonAsync($"/api/users/token/{provider}", userInfo);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<LoginInfo>();
    }

    public async Task<(User?, ErrorDetail?)> RegisterUserAsync(NewUserInfo userInfo)
    {
        if (string.IsNullOrEmpty(userInfo.UserName) || string.IsNullOrEmpty(userInfo.Password))
            return new(null,
                new ErrorDetail() { Title = "Invalid user data", Detail = "Username and password cannot be empty" });

        var response = await _httpClient.PostAsJsonAsync("/api/users/create", userInfo);

        var result = await HandleResponse(response);
        if (result is not null)
        {
            return (null, result);
        }

        var userResponse = await response.Content.ReadFromJsonAsync<User>();
        return (userResponse, null);
    }

    public (string?, ErrorDetail?) GetAccessTokenFromRefreshToken(string refreshToken, string userName)
    {
        if (_cache.TryGetValue(refreshToken, out string? accessToken))
        {
            return (accessToken, null);
        }
        
        return  (null, new ErrorDetail(){StatusCode = (int)HttpStatusCode.Forbidden, Title = "Invalid refresh token", Detail = "The refresh token is invalid, user needs to login again"});
    }

    public async Task<(LoginInfo?, ErrorDetail?)> RefreshAccessTokenAsync(string refreshToken, string userName)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/users/token/refresh", new { refreshToken, userName});
        var result = await HandleResponse(response);
        if (result is not null)
        {
            return (null, result);
        }
       
        var loginInfoResponse = await response.Content.ReadFromJsonAsync<LoginInfo>();
        if (loginInfoResponse is null) return (null, new ErrorDetail(){Title = "Internt fel", Detail = "Försök att logga in igen eller kontakta administratör vid återkommande problem"});

        TimeSpan duration = loginInfoResponse.RefreshTokenExpiration - DateTime.UtcNow;
        
        // We cache the access token with the refresh token as key so we do not have to call the API for every request
        _cache.Set(loginInfoResponse.RefreshToken, loginInfoResponse.Token, duration);
        return (loginInfoResponse, null);
    }

    public async Task<ErrorDetail?> HandleResponse(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<ErrorDetail>();
        }

        return null;
    }
}