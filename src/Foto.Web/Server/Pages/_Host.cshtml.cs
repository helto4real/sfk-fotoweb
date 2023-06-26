using System.Security.Claims;
using System.Security.Principal;
using Foto.Web.Client.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Foto.Web.Server.Pages;

public class IndexModel : PageModel
{
    private readonly ExternalProviders _socialProviders;

    public IndexModel(ExternalProviders socialProviders)
    {
        _socialProviders = socialProviders;
    }

    public string[] ProviderNames { get; set; } = default!;
    public AuthorizedUserInfo? CurrentUser { get; set; }

    public async Task OnGet()
    {
        ProviderNames = await _socialProviders.GetProviderNamesAsync();
        CurrentUser =
            new AuthorizedUserInfo(User?.Identity?.Name ?? string.Empty, UserRolesExtensions.RolesFromUser(User));
        
    }
}
