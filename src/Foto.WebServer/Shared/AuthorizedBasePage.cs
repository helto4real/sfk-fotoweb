using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Foto.WebServer.Shared;

[Authorize]
public class AuthorizedBasePage : BasePage
{
    [CascadingParameter]
    protected Task<AuthenticationState>? AuthenticationStateTask { get; set; }
    
    protected async Task<ClaimsPrincipal> GetCurrentUserAsync()
    {
        return (await AuthenticationStateTask!).User;
    } 
}