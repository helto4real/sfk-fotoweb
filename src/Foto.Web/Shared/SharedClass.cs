/* Shared classes can be referenced by both the Client and Server */
using System.ComponentModel.DataAnnotations;

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

public class LoginUserInfo
{
    [Required]
    public string Username { get; set; } = default!;

    [Required]
    public string Password { get; set; } = default!;
    
    public bool IsAdmin { get; set; } = false;
    
}

public class ImageItem
{
    public Guid Id { get; set; }
    [Required]
    public string Title { get; set; } = default!;
}



public class UserInfo
{
    [Required]
    public string Username { get; set; } = default!;
    
    public bool IsAdmin { get; set; } = false;
}

public class ExternalUserInfo
{
    [Required]
    public string Username { get; set; } = default!;

    [Required]
    public string ProviderKey { get; set; } = default!;
    
    public string UrlToken { get; set; } = default!;
}

public record AuthToken(string Token, bool IsAdmin);


public record UrlToken
{
    public Guid Id { get; init; }
    
    [Required] public string Token { get; set; } = default!;

    [Required] public  DateTime ExpirationDate { get; set; } = default!;
    [Required] public  UrlTokenType UrlTokenType { get; set; } = default!;
}

public enum UrlTokenType
{
    ResetPassword,
    ConfirmEmail,
    AllowAddUser,
    AllowAddImage,
};

public record NewTokenByType
{
    [Required]
    public UrlTokenType UrlTokenType { get; init; } = default!;
}

public record UrlTokenToken(string Token);