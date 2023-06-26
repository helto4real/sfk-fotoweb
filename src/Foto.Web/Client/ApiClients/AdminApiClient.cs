using System.Collections;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Foto.Web.Client.Authorization;
using Microsoft.AspNetCore.Components;

namespace Foto.Web.Client.ApiClients;

public class AdminApiClient : ApiClientBase
{
    private readonly HttpClient _client;

    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

    public AdminApiClient(
        HttpClient client,
        AuthorizationManager authManager,
        NavigationManager navigationManager) : base(authManager, navigationManager)
    {
        _client = client;
        _jsonOptions.Converters.Add(new JsonStringEnumConverter());
    }

    public async Task<IEnumerable<NewUserInfo>> GetAllUsers()
    {
        var response = await _client.GetAsync("api/admin/users");

        var result = await HandleResponse(response);
        if (result is not null) return Array.Empty<NewUserInfo>();

        var users = await response.Content.ReadFromJsonAsync<IEnumerable<NewUserInfo>>();

        return users ?? Array.Empty<NewUserInfo>();
    }

    public async Task<bool> UpdateUserAsync(UserInfo user)
    {
        var response = await _client.PutAsJsonAsync("api/admin/user", user);
        var result = await HandleResponse(response);
        // Todo handle error
        await HandleResponse(response);
        return response.IsSuccessStatusCode;
    }
    
    public async Task<bool> DeleteUserAsync(string username)
    {
        var response = await _client.DeleteAsync($"api/admin/user/{username}");
        var result = await HandleResponse(response);
        // Todo handle error
        await HandleResponse(response);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> CreateUserAsync(NewUserInfo newUser)
    {
        var response = await _client.PostAsJsonAsync("api/admin/user", newUser);
        var result = await HandleResponse(response);
        // Todo handle error
        return response.IsSuccessStatusCode;
    }

    public async Task<UserInfo?> GetUserAsync(string username)
    {
        var response = await _client.GetAsync($"api/admin/user/{username}");
        var result = await HandleResponse(response);
        // Todo handle error
        if (result is not null) return null;

        var user = await response.Content.ReadFromJsonAsync<UserInfo>();

        return user;
    }

    public async Task<bool> CreateUserAsync(string? username, string? password)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)) return false;

        var response = await _client.PostAsJsonAsync("api/auth/register",
            new NewUserInfo { UserName = username, Password = password });

        var result = await HandleResponse(response);
        // Todo handle error

        return response.IsSuccessStatusCode;
    }

    public async Task<List<UrlToken>> GetCurrentTokens()
    {
        var response = await _client.GetAsync($"api/admin/token/valid-tokens");

        var result = await HandleResponse(response);
        // Todo handle error
        if (result is not null) return new List<UrlToken>();

        return await response.Content.ReadFromJsonAsync<List<UrlToken>>(_jsonOptions) ?? new List<UrlToken>();
    }

    public async Task DeleteToken(Guid tokenId)
    {
        var response = await _client.DeleteAsync( $"api/admin/token/{tokenId}");
        var result = await HandleResponse(response);
        // Todo handle error
    }

    public async Task<UrlToken?> AddTokenByTokenType(UrlTokenType tokenType)
    {
        var response = await _client.PostAsJsonAsync("api/admin/token/addtokenbytype", new NewTokenByType { UrlTokenType = tokenType });

        var result = await HandleResponse(response);
        // Todo handle error
        if (result is not null) return null;
        
        return await response.Content.ReadFromJsonAsync<UrlToken>(_jsonOptions);
    }
}