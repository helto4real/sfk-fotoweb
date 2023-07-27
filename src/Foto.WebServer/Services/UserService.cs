using Foto.WebServer.Dto;
using Microsoft.Extensions.Options;

namespace Foto.WebServer.Services;

public class UserService : IUserService
{
    private readonly HttpClient _httpClient;
    private readonly ISignInService _signInService;

    public UserService(HttpClient httpClient, ISignInService signInService, IOptions<AppSettings> appSettings)
    {
        _httpClient = httpClient;
        _signInService = signInService;
        _appSettings = appSettings.Value;
        httpClient.BaseAddress = new Uri(_appSettings.FotoApiUrl);
        httpClient.DefaultRequestHeaders.Add("User-Agent", "FotoWebbServer");
    }

    private AppSettings _appSettings { get; }
    public async Task<UserInfo?> GetUserByUsernameAsync(string username)
    {
        var response =
            await _signInService.RefreshTokenOnExpired(async () =>
                await _httpClient.GetAsync($"api/users/user/{username}"));

        var user = await response.Content.ReadFromJsonAsync<UserInfo>();

        return user;
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

    public async Task<ErrorDetail?> HandleResponse(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode) return await response.Content.ReadFromJsonAsync<ErrorDetail>();

        return null;
    }
}