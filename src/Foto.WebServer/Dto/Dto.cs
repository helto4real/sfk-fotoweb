namespace Foto.WebServer.Dto;

public class NewUserInfo
{
    
    public string UserName { get; set; } = default!;

    public string Password { get; set; } = default!;

    public string Email { get; set; } = default!;
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    
    public bool IsAdmin { get; set; } = false;

    public string UrlToken { get; set; } = default!;
    
    public UserInfo ToUserInfo()
    {
        return new UserInfo
        {
            Username = UserName,
            IsAdmin = IsAdmin
        };
    }
}
public class UserInfo
{
    public string Username { get; set; } = default!;
    
    public bool IsAdmin { get; set; } = false;
}

public class ExternalUserInfo
{
    public string Username { get; set; } = default!;

    public string ProviderKey { get; set; } = default!;
    
    public string UrlToken { get; set; } = default!;
}

public record AuthToken(string Token, bool IsAdmin);

public class LoginUserInfo
{
    public string Username { get; set; } = default!;

    public string Password { get; set; } = default!;
    
    public bool IsAdmin { get; set; } = false;
    
}