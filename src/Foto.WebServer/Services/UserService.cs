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

    public async Task<ErrorDetail?> UpdateUserAsync(UserInfo user)
    {
        var response =
            await _signInService.RefreshTokenOnExpired(async () =>
                await _httpClient.PutAsJsonAsync("api/users/user", user));
        var result = await HandleResponse(response);
        return result;
    }
    
    public async Task<ErrorDetail?> ChangeLoginInfoAsync(UpdateLoginInfo loginInfo)
    {
        loginInfo.IsUserExternal = await _signInService.IsCurrentUserExternalAsync();
        var response =
            await _signInService.RefreshTokenOnExpired(async () =>
                await _httpClient.PutAsJsonAsync("api/users/logininfo", loginInfo));
        var result = await HandleResponse(response);
        return result;
    }

    public async Task SendPasswordResetEmail(string email)
    {
        var response =
            await _httpClient.PostAsJsonAsync("api/users/user/passwordreset/request", new EmailResetInfo(email));
        // We do not handle response since we do not want to leak information about the user, we log on server-side instead
    }

    public async Task<ErrorDetail?> ResetPassword(string email, string newPassword, string token)
    {
        var response =
            await _httpClient.PostAsJsonAsync("api/users/user/passwordreset/reset", new ResetPasswordInfo(email, token, newPassword));
        return await HandleResponse(response);
    }

    public async Task<ErrorDetail?> DeleteUserAsync(string username)
    {
        var response =
            await _signInService.RefreshTokenOnExpired(async () =>
                await _httpClient.DeleteAsync($"api/users/user/{username}"));
        var result = await HandleResponse(response);
        return result;
    }

    public async Task<(IEnumerable<User>?, ErrorDetail?)> GetAllUsers()
    {
        var response =
            await _signInService.RefreshTokenOnExpired(async () => await _httpClient.GetAsync("api/users"));
        var result = await HandleResponse(response);
        if (result is not null) return (null, result);
        var users = await response.Content.ReadFromJsonAsync<IEnumerable<User>>();
        return (users, null);
    }

    public async Task<(User?, ErrorDetail?)> CreateUserAsync(NewUserInfo user)
    {
        var response =
            await _signInService.RefreshTokenOnExpired(async () =>
                await _httpClient.PostAsJsonAsync("api/users/user", user));
        var result = await HandleResponse(response);
        if (result is not null) return (null, result);
        var users = await response.Content.ReadFromJsonAsync<User>();
        return (users, null);
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
    
    public async Task<ErrorDetail?> ConfirmEmailAsync(string token)
    {
        var escapedToken = Uri.EscapeDataString(token);
        var response = await _httpClient.GetAsync($@"api/public/confirmemail/{escapedToken}");
        var result = await HandleResponse(response);
        return result;
    }
}