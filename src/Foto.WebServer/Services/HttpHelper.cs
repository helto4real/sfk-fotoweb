using System.Net;
using Foto.WebServer.Authentication;
using Foto.WebServer.Dto;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Foto.WebServer.Services;

public class HttpHelper
{
    private readonly NavigationManager _navigationManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    TokenAuthorizationProvider _authManager;
    public HttpHelper(NavigationManager navigationManager, 
        AuthenticationStateProvider authenticationStateProvider,
        IHttpContextAccessor httpContextAccessor)
    {
        _navigationManager = navigationManager;
        _httpContextAccessor = httpContextAccessor;
        _authManager = (TokenAuthorizationProvider) authenticationStateProvider;
    }
    
    public async Task<ErrorDetail?> HandleResponse(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await _authManager.MarkUserAsLoggedOut();
                
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