using System.Security.Claims;
using FotoApi.Model;

namespace FotoApi.Infrastructure.Security.Authorization;

// A scoped service that exposes the current user information
public class CurrentUser
{
    public User? User { get; set; }
    public ClaimsPrincipal Principal { get; set; } = default!;

    public string Id => Principal.FindFirstValue(ClaimTypes.NameIdentifier)!;
    // public bool IsAdmin => Principal.IsInRole("admin");
    public bool IsAdmin => Principal.IsInRole("Admin");
}