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
 
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (AuthenticationStateTask is null)
            throw new NullReferenceException("authenticationStateTask is null");
        
        var claimsPrincipal = (await AuthenticationStateTask).User;
        if (claimsPrincipal?.Identity?.IsAuthenticated ?? false)
        {
            // We check if we have just logged in using a external provider
            // then we read teh cookie and mark the user as authenticated
            // This is cause we cannot mark the claim in the auth api call
            // This wil be refactored to use a .cshtml page instead of a API for external signin
            if (HttpContextAccessor!.HttpContext!.Request.Cookies.ContainsKey("externaltokeninfo"))
            {
                var tokenInfo = HttpContextAccessor.HttpContext.Request.Cookies["externaltokeninfo"];
                    // Delete the cookie so we do not expose the token longer than needed
                await ((TokenAuthorizationProvider) AuthenticationStateProvider!).SetTokenInformation(tokenInfo!);
                return;
            }
        }
        else
        {
            NavigationManager!.NavigateTo("login");
        }
    }
}