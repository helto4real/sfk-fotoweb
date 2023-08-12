namespace FotoApi.Features.HandleUsers.Dto;

public record UpdateLoginInfoRequest
{
    public string? CurrentPassword { get; init; }
    public string? NewPassword { get; init; }
    public string? NewEmail { get; init; }
    public bool IsUserExternal { get; init; }
}