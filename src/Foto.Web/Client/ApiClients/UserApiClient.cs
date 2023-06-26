using System.Net.Http.Json;
using Foto.Web.Client.Authorization;

namespace Foto.Web.Client.ApiClients;

public class UserApiClient
{
    private readonly HttpClient _client;

    public UserApiClient(HttpClient client)
    {
        _client = client;
    }

    public async Task<AuthorizedUserInfo?> GetUserAuthorizationInfoAsync()
    {
        var response = await _client.GetAsync("api/user/user_authorization");
        if (response.IsSuccessStatusCode) return await response.Content.ReadFromJsonAsync<AuthorizedUserInfo>();
        return null!;
    }

    public async Task<bool> LoginAsync(string? username, string? password, string urlToken)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)) return false;

        var response =
            await _client.PostAsJsonAsync("api/auth/login", new NewUserInfo { UserName = username, Password = password, UrlToken = urlToken });

        return response.IsSuccessStatusCode;
    }
    
    public async Task<bool> CreateUserAsync(string? username, string? password, string firsName, string lastName, string email, string token)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)) return false;

        var response = await _client.PostAsJsonAsync("api/auth/register",
            new NewUserInfo
            {
                UserName = username, 
                Password = password, 
                FirstName = firsName,
                LastName = lastName,
                UrlToken = token, Email = email
            });

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> LogoutAsync()
    {
        var response = await _client.PostAsync("api/auth/logout", null);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> ConfirmEmailAsync(string token)
    {
        var escapedToken = Uri.EscapeDataString(token);
        var result = await _client.GetAsync($@"api/public/confirmemail/{escapedToken}");

        return result.IsSuccessStatusCode;
    }
}