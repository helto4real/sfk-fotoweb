﻿namespace Foto.WebServer.Dto;

public class NewUserInfo
{
    
    public string UserName { get; set; } = default!;

    public string Password { get; set; } = default!;

    public string Email { get; set; } = default!;

    public string UrlToken { get; set; } = default!;
}

public class UserInfo
{
    public string Username { get; set; } = default!;
    public string Email { get; set; } = default!;
    public bool EmailConfirmed { get; set; }
    public List<string> Roles { get; set; } = new();
}

public class ExternalUserInfo
{
    public string UserName { get; set; } = default!;

    public string ProviderKey { get; set; } = default!;
    
    public string UrlToken { get; set; } = default!;
}

public class LoginUserInfo
{
    public string UserName { get; set; } = default!;

    public string Password { get; set; } = default!;
    
}