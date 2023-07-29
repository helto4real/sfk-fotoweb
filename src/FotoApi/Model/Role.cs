using Microsoft.AspNetCore.Identity;

namespace FotoApi.Model;

public class Role : IdentityRole
{
    public uint SortOrder { get; set; }
    public Role()
    {
    }

    public Role(string role) : base(role)
    {
    }
}