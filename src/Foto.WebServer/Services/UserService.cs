using Foto.WebServer.Dto;
using Microsoft.Extensions.Options;

namespace Foto.WebServer.Services;

public class UserService : ServiceBase, IUserService
{
    private readonly HttpClient _httpClient;
    private readonly ISignInService _signInService;

    public UserService(HttpClient httpClient, ISignInService signInService,
        IOptions<AppSettings> appSettings, ILogger<UserService> logger) : base(logger)
    {
        _httpClient = httpClient;
        _signInService = signInService;
        httpClient.BaseAddress = new Uri(appSettings.Value.FotoApiUrl);
        httpClient.DefaultRequestHeaders.Add("User-Agent", "FotoWebbServer");
    }

    public async Task<(UserInfo?, ErrorDetail?)> GetUserByUsernameAsync(string username)
    {
        var response =
            await _signInService.RefreshTokenOnExpired(async () =>
                await _httpClient.GetAsync($"api/users/user/{username}"));

        var result = await HandleResponse(response);
        if (result is not null) return (null, result);
        
        var user = await response.Content.ReadFromJsonAsync<UserInfo>();

        return (user, null);
    }

    public async Task<(UserInfo?, ErrorDetail?)> GetCurrentUserAsync()
    {
        var response =
            await _signInService.RefreshTokenOnExpired(async () =>
                await _httpClient.GetAsync($"api/users/user/current"));

        var result = await HandleResponse(response);
        if (result is not null) return (null, result);
        
        var user = await response.Content.ReadFromJsonAsync<UserInfo>();

        return (user, null);
    }

    public async Task<bool> UpdateUserAsync(UserInfo user)
    {
        var response =
            await _signInService.RefreshTokenOnExpired(async () =>
                await _httpClient.PutAsJsonAsync("api/users/user", user));
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteUserAsync(string username)
    {
        var response =
            await _signInService.RefreshTokenOnExpired(async () =>
                await _httpClient.DeleteAsync($"api/users/user/{username}"));
        return response.IsSuccessStatusCode;
    }

    public async Task<IEnumerable<User>> GetAllUsers()
    {
        var response =
            await _signInService.RefreshTokenOnExpired(async () => await _httpClient.GetAsync("api/users"));
        if (response.IsSuccessStatusCode)
        {
            var users = await response.Content.ReadFromJsonAsync<IEnumerable<User>>();
            return users ?? Array.Empty<User>();
        }

        return Array.Empty<User>();
    }

    public async Task<User?> CreateUserAsync(NewUserInfo user)
    {
        var response =
            await _signInService.RefreshTokenOnExpired(async () =>
                await _httpClient.PostAsJsonAsync("api/users/user", user));
        var users = await response.Content.ReadFromJsonAsync<User>();
        return users;
    }
    
    public async Task<(User?, ErrorDetail?)> PreCreateUserAsync(string? email)
    {
        var response = await _signInService.RefreshTokenOnExpired(async () =>
            await _httpClient.PostAsJsonAsync("api/users/precreate", new { email }));
        var result = await HandleResponse(response);
        if (result is not null) return (null, result);
        var user = await response.Content.ReadFromJsonAsync<User>();
        return (user, null);
    }
    
    public async Task<bool> ConfirmEmailAsync(string token)
    {
        var escapedToken = Uri.EscapeDataString(token);
        var result = await _httpClient.GetAsync($@"api/public/confirmemail/{escapedToken}");
        return result.IsSuccessStatusCode;
    }
}