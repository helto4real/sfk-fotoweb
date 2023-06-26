using Microsoft.AspNetCore.Identity;

namespace FotoApi.Model;

public class Role : IdentityRole
{
    public Role()
    {
    }

    public Role(string role) : base(role)
    {
    }
}