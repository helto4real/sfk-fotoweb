using System.Text.Json;

namespace Foto.Web.Server;

public class FotoApi
{
    private readonly HttpClient _client;

    public FotoApi(HttpClient client)
    {
        _client = client;
    }
    public async Task<AuthToken?> GetTokenAsync(LoginUserInfo newUserInfo)
    {
        var response = await _client.PostAsJsonAsync("/api/users/token", newUserInfo);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var token = await response.Content.ReadFromJsonAsync<AuthToken>();

        return token;
    }
    
    public async Task<AuthToken?> CreateUserAsync(NewUserInfo newUserInfo)
    {
        var response = await _client.PostAsJsonAsync("/api/users", newUserInfo);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await GetTokenAsync(new LoginUserInfo(){Username = newUserInfo.UserName, Password = newUserInfo.Password, IsAdmin = newUserInfo.IsAdmin});
    }
   
    public async Task<AuthToken?> GetOrCreateUserAsync(string provider, ExternalUserInfo userInfo)
    {
        var response = await _client.PostAsJsonAsync($"/api/users/token/{provider}", userInfo);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var token = await response.Content.ReadFromJsonAsync<AuthToken>();

        return token;
    }
}
