namespace Foto.WebServer.Authentication;

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
            UserRoles.User => "User",
            _ => throw new ArgumentOutOfRangeException(nameof(role), role, null)
        };
    }

    public static IEnumerable<string> AsEnumerable()
    {
        return Enum.GetValues<UserRoles>().Select(ToRoleName);
    }
}