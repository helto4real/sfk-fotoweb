namespace Foto.Web.Client.Authorization;


/// <summary>
///     This class needs to be included here because it is used in the generated code.
///     This means the server needs to have a dependency on the client.
/// </summary>
public class AuthorizedUserInfo
{
    public AuthorizedUserInfo(string username, string[] roles)
    {
        Username = username;
        Roles = roles;
    }
    
    public string Username { get; set; } = default!;
    public string[] Roles { get; set; } = default!;

    public bool IsInRole(string role) => Roles.Contains(role);
    
    public bool IsAuthorized => !string.IsNullOrEmpty(Username);
}