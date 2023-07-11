using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using Blazored.LocalStorage;
using Foto.WebServer.Dto;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Foto.WebServer.Services;

public class AdminService : IAdminService
{
    private readonly HttpClient _httpClient;
    private readonly IAuthService _authService;
    private readonly IHttpContextAccessor _accessor;
    private readonly AppSettings _appSettings;
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

    public AdminService(
        HttpClient httpClient, 
        IOptions<AppSettings> appSettings, 
        IAuthService authService, 
        IHttpContextAccessor accessor)
    {
        _httpClient = httpClient;
        _authService = authService;
        _accessor = accessor;
        _appSettings = appSettings.Value;
        _jsonOptions.Converters.Add(new JsonStringEnumConverter());
        httpClient.BaseAddress = new Uri(_appSettings.FotoApiUrl);
        httpClient.DefaultRequestHeaders.Add("User-Agent", "FotoWebbServer");
        
    }
    public async Task<UserInfo?> GetUserByUsernameAsync(string username)
    {
        var response = await _httpClient.GetAsync($"api/admin/user/{username}");

        var user = await response.Content.ReadFromJsonAsync<UserInfo>();

        return user;
    }

    public async Task<bool> UpdateUserAsync(UserInfo user)
    {
        var response = await _httpClient.PutAsJsonAsync("api/admin/user", user);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteUserAsync(string username)
    {
        var response = await _httpClient.DeleteAsync($"api/admin/user/{username}");
        return response.IsSuccessStatusCode;
    }

    public async Task<IEnumerable<User>> GetAllUsers()
    {
        var response = await _httpClient.GetAsync("api/admin/users");


        var users = await response.Content.ReadFromJsonAsync<IEnumerable<User>>();

        return users ?? Array.Empty<User>();
    }

    public async Task<User?> CreateUserAsync(NewUserInfo user)
    {
        var response = await _httpClient.PostAsJsonAsync("api/admin/user", user);
        var users = await response.Content.ReadFromJsonAsync<User>();
        return users;
    }

    public async Task<IEnumerable<UrlToken>> GetCurrentTokens()
    {
        var response = await _httpClient.GetAsync($"api/admin/token/valid-tokens");

        if (!response.IsSuccessStatusCode) return new List<UrlToken>();

        return await response.Content.ReadFromJsonAsync<List<UrlToken>>(_jsonOptions) ?? new List<UrlToken>();
    }

    public async ValueTask DeleteToken(Guid tokenId)
    {
        var response = await _httpClient.DeleteAsync( $"api/admin/token/{tokenId}");
    }

    public async Task<UrlToken?> AddTokenByTokenType(UrlTokenType tokenType)
    {
        var response = await _httpClient.PostAsJsonAsync("api/admin/token/addtokenbytype", new NewTokenByType { UrlTokenType = tokenType });

        // Todo handle error
        if (!response.IsSuccessStatusCode) return null;
        
        return await response.Content.ReadFromJsonAsync<UrlToken>(_jsonOptions);
    }

    public async Task<(User?, ErrorDetail?)> PreCreateUserAsync(string? email)
    {
        var response = await _httpClient.PostAsJsonAsync("api/admin/users/precreate", new {email = email});
        var result = await HandleResponse(response);
        if (result is not null)
        {
            return (null, result);
        }
        var user = await response.Content.ReadFromJsonAsync<User>();
        return (user, null);
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