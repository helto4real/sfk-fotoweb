using System.Net.Http.Json;
using Foto.Web.Client.Authorization;
using Microsoft.AspNetCore.Components;
using Todo.Web.Shared;

namespace Foto.Web.Client.ApiClients;

public class StBilderApiClient : ApiClientBase
{
    private readonly HttpClient _client;

    public StBilderApiClient(
        HttpClient client,
        AuthorizationManager authManager,
        NavigationManager navigationManager) : base(authManager, navigationManager)
    {
        _client = client;
    }

    public async Task<List<StBildInfo>> GetStBilder(bool useMyImages)
    {
        var response = await _client.GetAsync($"/api/stbilder?useMyImages={useMyImages}");
        if (response.IsSuccessStatusCode)
        {
            var stBilder = await response.Content.ReadFromJsonAsync<List<StBildInfo>>();
            if (stBilder is not null)
                return stBilder;
        }
        var result = await HandleResponse(response);
        // Todo handle error

        return new List<StBildInfo>();
    }
    
    public async Task<StBildInfo?> GetStBild(Guid stBildId)
    {
        var response = await _client.GetAsync($"/api/stbilder/{stBildId}");
        if (response.IsSuccessStatusCode)
        {
            var stBild = await response.Content.ReadFromJsonAsync<StBildInfo>();
            if (stBild is not null)
                return stBild;
        }
        var result = await HandleResponse(response);
        // Todo handle error

        return null;
    }

    public async Task UpdateStBild(StBildInfo stBild)
    {
        var response = await _client.PutAsJsonAsync($"/api/stbilder/{stBild.Id}", stBild);
        if (!response.IsSuccessStatusCode)
        {
            await HandleResponse(response);
            // Todo: fix error handling
        }
    }
}