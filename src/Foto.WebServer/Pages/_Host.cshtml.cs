using Foto.WebServer.Authentication;
using Foto.WebServer.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Foto.WebServer.Pages;

public class IndexModel : PageModel
{
    public Task OnGet()
    {
        return Task.CompletedTask;
    }
}