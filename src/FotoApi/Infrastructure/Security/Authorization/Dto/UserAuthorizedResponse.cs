namespace FotoApi.Infrastructure.Security.Authorization.Dto;

public record UserAuthorizedResponse
{
    public string UserName { get; init; } = default!;
    public string FirstName {get; init;} = default!;
    public string LastName {get; init;} = default!;
    public string Email {get; init;} = default!;
    public string Token { get; set; } = default!;
    public string RefreshToken { get; set; } = default!;
    public DateTime RefreshTokenExpiration { get; set; } = default!;
    public IReadOnlyCollection<string> Roles { get; init; } = default!;
}
    
    
