using System.Net;
using System.Net.Http.Json;
using Foto.Web.Client.Authorization;
using Microsoft.AspNetCore.Components;
using Todo.Web.Shared;

namespace Foto.Web.Client.ApiClients;

public class ApiClientBase
{
    private readonly AuthorizationManager _authManager;
    private readonly NavigationManager _navigationManager;

    public ApiClientBase(AuthorizationManager authManager, NavigationManager navigationManager)
    {
        _authManager = authManager;
        _navigationManager = navigationManager;
    }

    protected async Task<ErrorDetail?> HandleResponse(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await _authManager.Logout();
                _navigationManager.NavigateTo("/login", true);
            }
            else
            {
                return await response.Content.ReadFromJsonAsync<ErrorDetail>();
            }
        }

        return null;
    }
}