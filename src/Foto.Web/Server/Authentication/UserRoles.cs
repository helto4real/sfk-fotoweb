using System.Security.Principal;

namespace Foto.Web.Server;

public enum UserRoles
{
    Admin,
    User
}

public static class UserRolesExtensions
{
    public static string ToRoleName(this UserRoles role)
    {
        return role switch
        {
            UserRoles.Admin => "Admin",
            UserRoles.User => "ListUsers",
            _ => throw new ArgumentOutOfRangeException(nameof(role), role, null)
        };
    }
    
    public static IEnumerable<string> AsEnumerable()
    {
        return Enum.GetValues<UserRoles>().Select(ToRoleName);
    }
    
    public static string[] RolesFromUser(IPrincipal? user)
    {
        return user is null ? Array.Empty<string>() : AsEnumerable().Where(user.IsInRole).ToArray();
    }
}