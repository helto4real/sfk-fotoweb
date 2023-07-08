using Foto.WebServer.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Foto.WebServer.Shared;

public class AuthorizedBasePage : BasePage
{
    [CascadingParameter]
    protected Task<AuthenticationState>? AuthenticationStateTask { get; set; }
    
    [Inject]
    protected AuthenticationStateProvider? AuthenticationStateProvider { get; set; }
    [Inject]
    protected IHttpContextAccessor? HttpContextAccessor { get; set; }

    public bool IsConnected { get; private set; }
 
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            IsConnected = true;
            if (AuthenticationStateTask is null)
                throw new NullReferenceException("authenticationStateTask is null");
            
            var claimsPrincipal = (await AuthenticationStateTask).User;
            if (!claimsPrincipal?.Identity?.IsAuthenticated ?? false)
            {
                NavigationManager!.NavigateTo("login", true);
            }
        }
    }
}