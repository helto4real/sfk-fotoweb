using Microsoft.AspNetCore.Identity;

namespace FotoApi.Model;

public class User : IdentityUser
{
    public string RefreshToken { get; set; } = default!;
    public DateTime RefreshTokenExpirationDate { get; set; }
}