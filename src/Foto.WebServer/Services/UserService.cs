using Foto.WebServer.Authentication;
using Foto.WebServer.Dto;
using Microsoft.Extensions.Options;

namespace Foto.WebServer.Services;

public class UserService : IUserService
{
    private readonly HttpClient _httpClient;
    private AppSettings _appSettings { get; }
    public UserService(HttpClient httpClient, IOptions<AppSettings> appSettings)
    {
        _httpClient = httpClient;
        _appSettings = appSettings.Value;
        httpClient.BaseAddress = new Uri(_appSettings.FotoApiUrl);
        httpClient.DefaultRequestHeaders.Add("User-Agent", "FotoWebbServer");
    }
    public async Task<(User?, ErrorDetail?)> LoginAsync(LoginUserInfo loginInfo)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/users/token", loginInfo);
        var result = await HandleResponse(response);
        if (result is not null)
        {
            return (null, result);
        }
       
        var userResponse = await response.Content.ReadFromJsonAsync<User>();
        return (userResponse, null);
    }

    public async Task<(User?, ErrorDetail?)> RegisterUserAsync(NewUserInfo userInfo)
    {
        if (string.IsNullOrEmpty(userInfo.UserName) || string.IsNullOrEmpty(userInfo.Password)) return new (null, new ErrorDetail(){Title = "Invalid user data", Detail = "Username and password cannot be empty"});

        var response = await _httpClient.PostAsJsonAsync("/api/users/create", userInfo);

        var result = await HandleResponse(response);
        if (result is not null)
        {
            return (null, result);
        }
        var userResponse = await response.Content.ReadFromJsonAsync<User>();
        return (userResponse, null);
    }

    public async Task<User?> GetOrRegisterUserAsync(string provider, ExternalUserInfo userInfo)
    {
        var response = await _httpClient.PostAsJsonAsync($"/api/users/token/{provider}", userInfo);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<User>();
    }

    public async Task<User?> GetUserByAccessTokenAsync(string accessToken)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/users/bytoken", new TokenInfo(accessToken));
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var userResponse = await response.Content.ReadFromJsonAsync<User>();
        return userResponse;
    }

    public async Task<bool> ConfirmEmailAsync(string token)
    {
        var escapedToken = Uri.EscapeDataString(token);
        var result = await _httpClient.GetAsync($@"api/public/confirmemail/{escapedToken}");
        return result.IsSuccessStatusCode;
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