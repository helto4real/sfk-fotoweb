using Foto.Web.Client.Authorization;
using Microsoft.AspNetCore.Components;

namespace Foto.Web.Client.Shared;

public class AuthorizedBasePage : BasePage
{
    [Inject]
    protected AuthorizationManager AuthManager { get; set; } = default!;
    protected override Task OnInitializedAsync()
    {
        if (!AuthManager.IsAuthorized && NavigationManager is not null)
            NavigationManager.NavigateTo("login");
        return base.OnInitializedAsync();
    }
}