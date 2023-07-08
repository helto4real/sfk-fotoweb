using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Foto.WebServer.Pages;

public class IndexModel : PageModel
{
    public Task OnGet()
    {
        return Task.CompletedTask;
    }
    
}